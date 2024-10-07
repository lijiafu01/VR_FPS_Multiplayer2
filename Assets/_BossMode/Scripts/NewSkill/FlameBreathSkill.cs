using Fusion;
using UnityEngine;

public class FlameBreathSkill : NetworkBehaviour, IBossSkill
{
    public string SkillName => "Flame Breath";

    [SerializeField]
    private float cooldown = 15f;
    public float Cooldown => cooldown;

    [SerializeField]
    private float castingDuration = 2f; // Thời gian kỹ năng diễn ra
    public float CastingDuration => castingDuration;

    [Networked]
    private TickTimer cooldownTimer { get; set; }

    [Networked]
    private TickTimer castingTimer { get; set; }

    [Networked]
    private NetworkBool isCastingNetworked { get; set; }

    public bool IsCasting => isCastingNetworked;

    public bool IsOnCooldown => !cooldownTimer.ExpiredOrNotRunning(Runner);

    public event System.Action OnSkillStart;
    public event System.Action OnSkillEnd;

    [SerializeField]
    private GameObject flameBreathObject; // Object con (tia laser)

    [SerializeField]
    private float rotationSpeed = 90f; // Tốc độ quay (độ mỗi giây)

    private Animator animator;

    [Networked]
    private NetworkBool isRotating { get; set; }

    [Networked]
    private float rotationAngle { get; set; }

    [Networked]
    private Quaternion bossRotation { get; set; }

    void Awake()
    {
        animator = GetComponentInParent<Animator>();

        // Đảm bảo object con (tia laser) ban đầu bị tắt
        if (flameBreathObject != null)
        {
            flameBreathObject.SetActive(false);
        }
    }

    public void ActivateSkill(Transform target)
    {
        if (Object.HasStateAuthority && !IsOnCooldown && !IsCasting)
        {
            isCastingNetworked = true;
            castingTimer = TickTimer.CreateFromSeconds(Runner, CastingDuration);
            cooldownTimer = TickTimer.CreateFromSeconds(Runner, Cooldown);

            OnSkillStart?.Invoke();

            // Kích hoạt animation Skill3
            animator.SetTrigger("Skill3");

            // Gọi RPC để kích hoạt hiệu ứng trên tất cả các client
            RPC_StartFlameBreath();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_StartFlameBreath()
    {
        // Kích hoạt object con (tia laser)
        if (flameBreathObject != null)
        {
            flameBreathObject.SetActive(true);
        }

        // Bắt đầu quay
        isRotating = true;
        rotationAngle = 0f;

        // Khởi tạo góc quay của Boss
        bossRotation = transform.parent.rotation;
    }

    public void FixedUpdateSkill()
    {
        if (isCastingNetworked)
        {
            // Xử lý quay
            if (isRotating)
            {
                RotateBoss();
            }

            // Nếu cần thiết, bạn có thể giữ kiểm tra castingTimer ở đây
            // Nếu không, có thể bỏ qua
        }

        // Áp dụng góc quay cho Boss trên tất cả các client
        transform.parent.rotation = bossRotation;
    }

    void RotateBoss()
    {
        float rotationStep = rotationSpeed * Runner.DeltaTime;

        // Cập nhật góc quay
        rotationAngle += rotationStep;

        // Tính toán góc quay mới
        Quaternion deltaRotation = Quaternion.Euler(0f, rotationStep, 0f);
        bossRotation *= deltaRotation;

        // Áp dụng góc quay cho Boss
        transform.parent.rotation = bossRotation;

        // Kiểm tra nếu đã quay đủ 180 độ
        if (rotationAngle >= 390f)
        {
            Debug.Log("boss7_2_ quay xong 180");
            animator.SetTrigger("Idle");

            // Kết thúc quay
            isRotating = false;

            // Gọi RPC để tắt tia laser
            RPC_EndFlameBreath();

            // **Đặt isCastingNetworked về false và gọi OnSkillEnd**
            if (Object.HasStateAuthority)
            {
                isCastingNetworked = false;
                OnSkillEnd?.Invoke();
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_EndFlameBreath()
    {
        // Tắt object con (tia laser)
        if (flameBreathObject != null)
        {
            flameBreathObject.SetActive(false);
        }
    }
}
