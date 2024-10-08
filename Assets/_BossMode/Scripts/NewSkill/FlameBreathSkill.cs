using Fusion;
using UnityEngine;

public class FlameBreathSkill : NetworkBehaviour, IBossSkill
{
    public string SkillName => "Flame Breath";

    [SerializeField]
    private float cooldown = 15f;
    public float Cooldown => cooldown;

    [SerializeField]
    private float castingDuration = 2f;
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

    private Animator animator;

    void Awake()
    {
        animator = GetComponentInParent<Animator>();

        // Đảm bảo object con (tia laser) ban đầu bị tắt
        if (flameBreathObject != null)
        {
            flameBreathObject.SetActive(false);
        }

        Debug.Log("Awake: Animator initialized and flameBreathObject set inactive.");
    }

    public void ActivateSkill(Transform target)
    {

        if (Object.HasStateAuthority && !IsOnCooldown && !IsCasting)
        {
            isCastingNetworked = true;
            castingTimer = TickTimer.CreateFromSeconds(Runner, CastingDuration);
            cooldownTimer = TickTimer.CreateFromSeconds(Runner, Cooldown);

            // Đặt giá trị ban đầu cho currentRotation
            currentRotation = transform.parent.rotation;
            OnSkillStart?.Invoke();
            animator.SetTrigger("Skill3");
            Debug.Log("ActivateSkill: Skill activated.");

            RPC_StartFlameBreath();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_StartFlameBreath()
    {
        if (flameBreathObject != null)
        {
            flameBreathObject.SetActive(true);
        }

        Debug.Log("RPC_StartFlameBreath: Flame breath started.");
    }
    [Networked]
    private Quaternion currentRotation { get; set; }
    [SerializeField]
    private float rotationSpeed = 90f; // Tốc độ quay (độ mỗi giây)
    public void FixedUpdateSkill()
    {
        if (Object.HasStateAuthority)
        {
            if (IsCasting && castingTimer.Expired(Runner))
            {
                RPC_EndFlameBreath();
                // Gán giá trị cho biến mạng nội bộ
                isCastingNetworked = false;
                Debug.Log("boss7_da ket thuc chieu 3");
                OnSkillEnd?.Invoke();
            }
           if(isCastingNetworked)
           {
                // Tính toán góc quay mới dựa trên tốc độ và thời gian giữa các frame
                float rotationStep = rotationSpeed * Runner.DeltaTime;
                Quaternion deltaRotation = Quaternion.Euler(0f, rotationStep, 0f);

                // Cập nhật góc quay
                currentRotation *= deltaRotation;

                // Áp dụng góc quay mới cho object
                transform.parent.rotation = currentRotation;
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_EndFlameBreath()
    {
        if (flameBreathObject != null)
        {
            flameBreathObject.SetActive(false);
        }

        Debug.Log("RPC_EndFlameBreath: Flame breath ended.");
    }
}
