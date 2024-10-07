using Fusion;
using UnityEngine;

public class ThrowBallSkill : NetworkBehaviour, IBossSkill
{
    [SerializeField] private NetworkPrefabRef fireballPrefab;
    [SerializeField] private Transform firePoint;
    public string SkillName => "Throw Ball";

    [SerializeField]
    private float cooldown = 10f;
    public float Cooldown => cooldown;

    [SerializeField]
    private float castingDuration = 1f;
    public float CastingDuration => castingDuration;

    [Networked]
    private TickTimer cooldownTimer { get; set; }

    [Networked]
    private TickTimer castingTimer { get; set; }

    // Biến mạng nội bộ
    [Networked]
    private NetworkBool isCastingNetworked { get; set; }

    // Triển khai thuộc tính IsCasting từ IBossSkill
    public bool IsCasting => isCastingNetworked;

    public bool IsOnCooldown => !cooldownTimer.ExpiredOrNotRunning(Runner);

    public event System.Action OnSkillStart;
    public event System.Action OnSkillEnd;

    // Các biến cần thiết cho kỹ năng
    // Ví dụ: Prefab của quả bóng, vị trí ném, v.v.
    [SerializeField]
    private NetworkPrefabRef ballPrefab;

    [SerializeField]
    private Transform throwPoint;

    private Animator animator;

    void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    public void ActivateSkill(Transform target)
    {
        Debug.Log("boss7_da bat dau chieu 1");
        if (Object.HasStateAuthority && !IsOnCooldown && !IsCasting)
        {
            // Gán giá trị cho biến mạng nội bộ
            isCastingNetworked = true;

            castingTimer = TickTimer.CreateFromSeconds(Runner, CastingDuration);
            cooldownTimer = TickTimer.CreateFromSeconds(Runner, Cooldown);

            OnSkillStart?.Invoke();
            // Gọi RPC để đồng bộ hóa nếu cần
            RPC_ActivateSkill(target.position);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_ActivateSkill(Vector3 targetPosition)
    {
        Debug.Log("boss7_da bat dau chieu 1_RPC");

        // Kích hoạt animation nếu cần
        animator.SetTrigger("Skill1");

        // Sinh quả cầu lửa trên State Authority
        if (Object.HasStateAuthority)
        {
            Debug.Log("boss7_check HasStateAuthority");
            NetworkObject fireballObject = Runner.Spawn(fireballPrefab, firePoint.position, Quaternion.identity, Object.InputAuthority);

            // Truyền vị trí mục tiêu cho quả cầu lửa
            var fireballScript = fireballObject.GetComponent<FireballNetworked>();
            if (fireballScript != null)
            {
                fireballScript.SetTargetPosition(targetPosition);
            }
        }
    }

    public void FixedUpdateSkill()
    {
        if (Object.HasStateAuthority)
        {
            if (IsCasting && castingTimer.Expired(Runner))
            {
                // Gán giá trị cho biến mạng nội bộ
                isCastingNetworked = false;
                Debug.Log("boss7_da ket thuc chieu 1");
                OnSkillEnd?.Invoke();
            }
        }
    }
}
