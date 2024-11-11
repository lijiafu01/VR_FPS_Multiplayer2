using Fusion;
using UnityEngine;

public class Skill4 : NetworkBehaviour, IBossSkill
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private float _BuffHPPercent = 0.3f;
    [SerializeField] BossNetworked _BossNetworked;
    [SerializeField] private Transform skill4ActionPoint;
    [SerializeField] private NetworkObject _HpBuffVFX;
    // Tên của kỹ năng
    public string SkillName => "Skill4";

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

    // Các biến khác cần thiết cho logic của kỹ năng
    // Ví dụ:
    // [SerializeField]
    // private GameObject effectPrefab;

    void Awake()
    {
        // Khởi tạo các thành phần cần thiết
        animator = GetComponentInParent<Animator>();

        // Khởi tạo các biến khác (nếu cần)
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
            SpawnVFX();
            SetAnimator_RPC();
            Invoke("BuffBossHP", 1f);



        }
    }
    void BuffBossHP()
    {
        if (Object.HasStateAuthority)
        {
            int maxHealth = _BossNetworked.MaxHealth;
            int tenPercentOfMaxHealth = Mathf.RoundToInt(maxHealth * _BuffHPPercent);
            _BossNetworked.CurrentHealth += tenPercentOfMaxHealth;
        }

    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void SetAnimator_RPC()
    {
        animator.SetTrigger("Skill4");
        Invoke("PlaySound", 1f);
    }
    void PlaySound()
    {
        audioSource.Play();
    }
    void SpawnVFX()
    {
        Runner.Spawn(_HpBuffVFX, skill4ActionPoint.position,Quaternion.identity);
    }


    public void FixedUpdateSkill()
    {
        if (Object.HasStateAuthority)
        {
            // Kiểm tra nếu kỹ năng đang thi triển và thời gian thi triển đã hết
            if (IsCasting && castingTimer.Expired(Runner))
            {
                // Kết thúc kỹ năng
                isCastingNetworked = false;

                // Gọi sự kiện kết thúc kỹ năng
                OnSkillEnd?.Invoke();


            }

            // Thực hiện logic của kỹ năng trong quá trình thi triển (nếu cần)
            if (IsCasting)
            {
                // Thực hiện logic liên tục trong quá trình thi triển
            }
        }
    }


}
