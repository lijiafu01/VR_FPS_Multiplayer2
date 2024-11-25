using Fusion;
using UnityEngine;

public class Boss3Skill3 : NetworkBehaviour, IBossSkill
{
    [SerializeField] private Transform skill3ActionPoint;
    [SerializeField] private NetworkObject _FlashFX;
    // Tên của kỹ năng
    public string SkillName => "Skill3";
    // Thời gian hồi chiêu (cooldown) của kỹ năng
    [SerializeField]
    private float cooldown = 15f;
    public float Cooldown => cooldown;
    // Thời gian thi triển kỹ năng (casting duration)
    [SerializeField]
    private float castingDuration = 2f;
    public float CastingDuration => castingDuration;
    // Các biến mạng để đồng bộ hóa thời gian hồi chiêu và thi triển
    [Networked]
    private TickTimer cooldownTimer { get; set; }
    [Networked]
    private TickTimer castingTimer { get; set; }
    // Biến mạng để đồng bộ trạng thái đang thi triển kỹ năng
    [Networked]
    private NetworkBool isCastingNetworked { get; set; }
    public bool IsCasting => isCastingNetworked;
    public bool IsOnCooldown => !cooldownTimer.ExpiredOrNotRunning(Runner);
    // Sự kiện khi kỹ năng bắt đầu và kết thúc
    public event System.Action OnSkillStart;
    public event System.Action OnSkillEnd;
    // Tham chiếu đến Animator của boss (nếu cần)
    private Animator animator;
    private Transform _target = null;
    void Awake()
    {
        // Khởi tạo các thành phần cần thiết
        animator = GetComponentInParent<Animator>();
    }

    public void ActivateSkill(Transform target)
    {
        // Kiểm tra quyền điều khiển và trạng thái kỹ năng
        if (Object.HasStateAuthority && !IsOnCooldown && !IsCasting)
        {
            // Đặt trạng thái kỹ năng
            isCastingNetworked = true;
            castingTimer = TickTimer.CreateFromSeconds(Runner, CastingDuration);
            cooldownTimer = TickTimer.CreateFromSeconds(Runner, Cooldown);
            // Gọi sự kiện bắt đầu kỹ năng
            OnSkillStart?.Invoke();
            _target = target;
            Invoke("SpawnVFX", 0.45f);
            SetAnimator_RPC();
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void SetAnimator_RPC()
    {
        animator.SetTrigger("Skill3");
    }
    void SpawnVFX()
    {
        if(Object.HasStateAuthority)
        {
            Runner.Spawn(_FlashFX, skill3ActionPoint.position, transform.parent.rotation);
        }
    }
    public void FixedUpdateSkill()
    {
        if (Object.HasStateAuthority)
        {
            if (IsCasting && castingTimer.Expired(Runner))
            {
                isCastingNetworked = false;
                OnSkillEnd?.Invoke();
            }
            if (IsCasting)
            {
            }
        }
    }
}
