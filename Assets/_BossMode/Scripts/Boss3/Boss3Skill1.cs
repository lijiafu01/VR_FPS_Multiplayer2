using Fusion;
using UnityEngine;

public class Boss3Skill1 : NetworkBehaviour, IBossSkill
{
    public GameObject OwnSword;
    [SerializeField] private Transform skill1ActionPoint;
    [SerializeField] private NetworkObject _SwordPrefab;
    // Tên của kỹ năng
    public string SkillName => "Skill1";

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
    private Transform _target = null;
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
            _target = target;
            SetAnimator_RPC();
            Invoke("SpawnObject", 1f);


        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void SetAnimator_RPC()
    {
        animator.SetTrigger("Skill1");
        Invoke("SetHideObject",0.7f);

    }
    void SetHideObject()
    {
        OwnSword.SetActive(false);
    }
    void SpawnObject()
    {
        if (Object.HasStateAuthority)
        {
            // Spawn thanh chính
            NetworkObject mainSword = Runner.Spawn(_SwordPrefab, skill1ActionPoint.position, transform.parent.rotation);
            Boss3Sword mainBoss3Sword = mainSword.GetComponent<Boss3Sword>();
            BossNetworked bossNetworked = GetComponentInParent<BossNetworked>();
            mainBoss3Sword.FlyObject(_target);
            mainBoss3Sword.Init(bossNetworked, this);

            // Spawn các thanh phụ tại các vị trí x cố định
            SpawnAdditionalSwordAtOffset(1, bossNetworked,15);
            SpawnAdditionalSwordAtOffset(2, bossNetworked,-15);
            SpawnAdditionalSwordAtOffset(-1, bossNetworked,30);
            SpawnAdditionalSwordAtOffset(-2, bossNetworked,-30);
        }
    }
    void SpawnAdditionalSwordAtOffset(float zOffset, BossNetworked bossNetworked,float y)
    {
        // Tính toán vị trí spawn của thanh phụ trên trục x với offset
        Vector3 spawnPosition = skill1ActionPoint.position + new Vector3(zOffset, 0, 0);

        // Spawn thanh phụ tại vị trí với offset đã tính toán
        NetworkObject sideSword = Runner.Spawn(_SwordPrefab, spawnPosition, transform.parent.rotation);
        Boss3Sword sideBoss3Sword = sideSword.GetComponent<Boss3Sword>();
        sideBoss3Sword.Init(bossNetworked, this);
        sideBoss3Sword.FlyObject2(_target,y);



        // Nếu không cần thêm logic di chuyển hay mục tiêu, không cần chỉnh thêm gì ở đây
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
