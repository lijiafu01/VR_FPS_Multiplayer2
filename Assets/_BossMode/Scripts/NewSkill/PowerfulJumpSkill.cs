using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

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

    void Awake()
    {
        animator = GetComponentInParent<Animator>();
        rbBoss = GetComponentInParent<Rigidbody>();
    }

    public void ActivateSkill(Transform target)
    {
        Debug.Log("Boss bắt đầu kỹ năng nhảy");
        if (Object.HasStateAuthority && !IsOnCooldown && !IsCasting)
        {
            isCastingNetworked = true;
            isJumping = true;
            castingTimer = TickTimer.CreateFromSeconds(Runner, CastingDuration);
            cooldownTimer = TickTimer.CreateFromSeconds(Runner, Cooldown);

            OnSkillStart?.Invoke();

            animator.SetTrigger("Skill2");

            // Tính toán và áp dụng vận tốc nhảy
            JumpTowardsTarget(target.position);
        }
    }

    void JumpTowardsTarget(Vector3 targetPosition)
    {
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
        Vector3 velocity = direction * initialSpeed * Mathf.Cos(45 * Mathf.Deg2Rad) + Vector3.up * initialSpeed * Mathf.Sin(45 * Mathf.Deg2Rad);

        // Áp dụng vận tốc cho Rigidbody
        rbBoss.velocity = velocity;
    }

    public void HandleCollision(Collision collision)
    {
        Debug.Log("boss7_check va cham ground_1");
        if (Object.HasStateAuthority && isJumping)
        {
            // Kiểm tra nếu va chạm với mặt đất
            if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("LocalPlayer"))
            {
                animator.SetTrigger("Idle");

                Debug.Log("boss7_check va cham ground_2");
                // Gọi phương thức tạo lực đẩy
                //CreateShockwave();
                RPC_PlayShockwaveEffect();


            }
        }
    }


    void CreateShockwave()
    {
        float explosionRadius = 10f;
        float explosionForce = 6f;
        Vector3 explosionPosition = transform.parent.position;

        // Tìm các đối tượng trong bán kính vụ nổ
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, explosionRadius);

        foreach (Collider hit in colliders)
        {
            Debug.Log("boss7_1_Đã phát hiện Collider: " + hit.gameObject.name);

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null && rb != rbBoss)
            {
                rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, 1f, ForceMode.Impulse);
               
                Debug.Log("boss7_1_Đã áp dụng lực đẩy lên: " + rb.gameObject.name);
            }
        }

        // Gọi RPC để sinh hiệu ứng nổ trên tất cả các client
        //RPC_PlayShockwaveEffect();
    }

   
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_PlayShockwaveEffect()
    {
        Instantiate(EndJumpVFX, transform.position, Quaternion.identity);
        CreateShockwave();

        // Sinh hiệu ứng nổ tại vị trí của boss
        // Ví dụ: Instantiate(shockwaveEffectPrefab, transform.position, Quaternion.identity);
    }

    public void FixedUpdateSkill()
    {
        if (Object.HasStateAuthority)
        {
            if (IsCasting && castingTimer.Expired(Runner))
            {
                isJumping = false;

                isCastingNetworked = false;
                OnSkillEnd?.Invoke();
                Debug.Log("Boss kết thúc kỹ năng nhảy");
            }
        }
    }
}
