using Fusion;
using UnityEngine;
public class Skill3 : NetworkBehaviour, IBossSkill
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField]
    private int spawnNum = 5;
    [SerializeField] private NetworkObject _spawnVFX;
    [SerializeField] private float spawnRadius;
    [SerializeField] private NetworkObject _treantMinionPrefab;
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
            SetAnimator_RPC();
            Invoke("SpawnObject", 2f);
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void SetAnimator_RPC()
    {
        animator.SetTrigger("Skill3");
        Invoke("PlaySound", 2.01f);
    }
    void PlaySound()
    {
        audioSource.Play();
    }
    void SpawnObject()
    {
        SpawnTreant();
    }
    public void SpawnTreant()
    {
        if (!Object.HasStateAuthority) return;
        for (int i = 1; i <= spawnNum; i++)
        {
            // Lấy vị trí ngẫu nhiên trong một vòng tròn (x, z) với bán kính đã chỉ định
            Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
            // Lấy vị trí của parent làm tâm
            Vector3 parentPosition = transform.position;
            // Thiết lập vị trí với x và z từ vòng tròn ngẫu nhiên cộng với vị trí parent, y cố định ở -3.5
            Vector3 spawnPosition = new Vector3(parentPosition.x + randomPos.x, 3f, parentPosition.z + randomPos.y);
            // Kiểm tra nếu khoảng cách từ spawnPosition tới tâm (0, 0, 0) vượt quá radius cho phép
            float distanceFromCenter = Vector3.Distance(spawnPosition, Vector3.zero);
            if (distanceFromCenter > 45f)
            {
                // Nếu vị trí vượt quá phạm vi, bỏ qua lần sinh này và tiếp tục vòng lặp
                continue;
            }
            // Sinh ra đối tượng _spikePrefab tại vị trí spawnPosition
            Runner.Spawn(_treantMinionPrefab, spawnPosition, Quaternion.identity);
            Runner.Spawn(_spawnVFX, spawnPosition, Quaternion.identity);
        }
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
