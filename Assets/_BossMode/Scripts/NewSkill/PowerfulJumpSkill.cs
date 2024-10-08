using Fusion;
using multiplayerMode;
using UnityEngine;

public class PowerfulJumpSkill : NetworkBehaviour, IBossSkill
{
    public GameObject StartJumpVFX;
    public GameObject EndJumpVFX;
    public string SkillName => "Powerful Jump";

    [SerializeField]
    private float cooldown = 15f;
    public float Cooldown => cooldown;

    [SerializeField]
    private float castingDuration = 3f;
    public float CastingDuration => castingDuration;

    [Networked]
    private TickTimer cooldownTimer { get; set; }

    [Networked]
    private TickTimer castingTimer { get; set; }

    [Networked]
    private NetworkBool isCastingNetworked { get; set; }

    [Networked]
    private NetworkBool isJumping { get; set; }

    public bool IsCasting => isCastingNetworked;

    public bool IsOnCooldown => !cooldownTimer.ExpiredOrNotRunning(Runner);

    public event System.Action OnSkillStart;
    public event System.Action OnSkillEnd;

    [SerializeField]
    private float jumpForce = 10f;

    private Rigidbody rbBoss;
    private Animator animator;

    private float timeOfFlight; // Thời gian bay tổng cộng

    [SerializeField]
    private float effectTime = 2f; // Thời gian trước khi chạm đất để kích hoạt hiệu ứng

    private TickTimer effectTimer;

    private Vector3 targetPosition;

    [SerializeField]
    private float minJumpDistance = 1f; // Khoảng cách tối thiểu để thực hiện kỹ năng nhảy

    [SerializeField]
    private Transform effectParent; // Kéo đối tượng con vào đây trong Inspector

    void Awake()
    {
        animator = GetComponentInParent<Animator>();
        rbBoss = GetComponentInParent<Rigidbody>();
    }

    public void ActivateSkill(Transform target)
    {
        Debug.Log("Boss bắt đầu kiểm tra kỹ năng nhảy");

        if (Object.HasStateAuthority && !IsOnCooldown && !IsCasting)
        {
            // Tính toán khoảng cách ngang trên mặt phẳng XZ
            Vector3 startPosition = transform.position;
            Vector3 targetPositionXZ = new Vector3(target.position.x, startPosition.y, target.position.z);
            float distance = Vector3.Distance(startPosition, targetPositionXZ);

            // Kiểm tra nếu khoảng cách nhỏ hơn khoảng cách tối thiểu
            if (distance < minJumpDistance)
            {
                Debug.Log("Khoảng cách quá gần, hủy kỹ năng nhảy");

                // Kết thúc kỹ năng mà không thực hiện nhảy
                isCastingNetworked = false;
                OnSkillEnd?.Invoke();

                // Đặt animation về trạng thái Idle nếu cần
                animator.SetTrigger("Idle");

                // Đặt thời gian hồi chiêu nếu cần thiết
                cooldownTimer = TickTimer.CreateFromSeconds(Runner, Cooldown);

                return;
            }

            // Nếu khoảng cách đủ lớn, thực hiện kỹ năng nhảy
            isCastingNetworked = true;
            isJumping = true;
            castingTimer = TickTimer.CreateFromSeconds(Runner, CastingDuration);
            cooldownTimer = TickTimer.CreateFromSeconds(Runner, Cooldown);

            // Đặt lại effectTimer
            effectTimer = TickTimer.None;

            OnSkillStart?.Invoke();

            animator.SetTrigger("Skill2");

            // Tính toán và áp dụng vận tốc nhảy
            JumpTowardsTarget(target.position);
        }
    }

    void JumpTowardsTarget(Vector3 targetPos)
    {
        // Lưu trữ targetPosition
        targetPosition = targetPos;

        // Tính toán khoảng cách ngang trên mặt phẳng XZ
        Vector3 startPosition = transform.position;
        Vector3 horizontalDisplacement = new Vector3(targetPosition.x - startPosition.x, 0, targetPosition.z - startPosition.z);
        float distance = horizontalDisplacement.magnitude;

        // Gia tốc trọng trường
        float gravity = Mathf.Abs(Physics.gravity.y);

        // Tính toán vận tốc ban đầu cần thiết
        float initialSpeed = Mathf.Sqrt(distance * gravity);

        // Tính toán hướng vận tốc ban đầu
        Vector3 direction = horizontalDisplacement.normalized;

        // Tính toán vector vận tốc ban đầu ở góc 45 độ
        float angle = 45f * Mathf.Deg2Rad;
        Vector3 velocity = direction * initialSpeed * Mathf.Cos(angle) + Vector3.up * initialSpeed * Mathf.Sin(angle);

        // Áp dụng vận tốc cho Rigidbody
        rbBoss.velocity = velocity;

        // Tính toán vận tốc theo trục Y
        float v0y = initialSpeed * Mathf.Sin(angle);

        // Tính toán thời gian bay tổng cộng
        float g = gravity;
        timeOfFlight = (2 * v0y) / g;

        // Kiểm tra nếu effectTime lớn hơn hoặc bằng thời gian bay
        float t_effect;
        if (effectTime >= timeOfFlight)
        {
            // Nếu effectTime >= timeOfFlight, kích hoạt hiệu ứng ngay lập tức hoặc không kích hoạt
            t_effect = 0f;
            Debug.LogWarning("effectTime lớn hơn hoặc bằng thời gian bay. Hiệu ứng sẽ được kích hoạt ngay lập tức.");
        }
        else
        {
            // Tính toán thời điểm kích hoạt hiệu ứng
            t_effect = timeOfFlight - effectTime;
        }

        // Bắt đầu bộ đếm thời gian để kích hoạt hiệu ứng
        if (Object.HasStateAuthority)
        {
            effectTimer = TickTimer.CreateFromSeconds(Runner, t_effect);
        }
    }

    public void HandleCollision(Collision collision)
    {
        Debug.Log("Kiểm tra va chạm với mặt đất");
        if (Object.HasStateAuthority && isJumping)
        {
            // Kiểm tra nếu va chạm với mặt đất
            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("LocalPlayer"))
            {
                animator.SetTrigger("Idle");

                Debug.Log("Boss đã chạm đất");
                // Gọi phương thức tạo lực đẩy
                RPC_PlayShockwaveEffect();

                // Đặt lại isJumping
                isJumping = false;
            }
        }
    }
    [SerializeField]
    private float explosionRadius = 15f;
    [SerializeField]
    private float explosionForce = 8f;
    [SerializeField]
    private float upwardsModifier = 10f; // Cho phép điều chỉnh trong Inspector

    void CreateShockwave()
    {
        Vector3 explosionPosition = targetPosition;

        // Tìm các đối tượng trong bán kính vụ nổ
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);
       
        foreach (Collider hit in colliders)
        {
            PlayerController playerController = hit.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage_Boss(15);
            }
            Debug.Log("Đã phát hiện Collider: " + hit.gameObject.name);

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null && rb != rbBoss)
            {
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
               
            }
        }
    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_PlayShockwaveEffect()
    {
        GameObject effect = Instantiate(EndJumpVFX, effectParent.position, Quaternion.identity, effectParent);
        CreateShockwave();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_PlayPreLandingEffect()
    {
        // Sinh hiệu ứng trước khi hạ cánh
        Instantiate(StartJumpVFX, targetPosition, Quaternion.identity);
    }

    public void FixedUpdateSkill()
    {
        if (Object.HasStateAuthority)
        {
            // Kiểm tra xem có cần kích hoạt hiệu ứng trước khi hạ cánh không
            if (isJumping && effectTimer.Expired(Runner))
            {
                // Kích hoạt hiệu ứng
                RPC_PlayPreLandingEffect();

                // Vô hiệu hóa timer sau khi kích hoạt
                effectTimer = TickTimer.None;
            }

            if (IsCasting && castingTimer.Expired(Runner))
            {
                isCastingNetworked = false;
                OnSkillEnd?.Invoke();
                Debug.Log("Boss kết thúc kỹ năng nhảy");
            }
        }
    }
}
