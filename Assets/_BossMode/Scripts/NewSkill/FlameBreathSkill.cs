using Fusion;
using multiplayerMode;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FlameBreathSkill : NetworkBehaviour, IBossSkill
{
    [SerializeField]
    private AudioSource _dragonRoar;

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

    [SerializeField]
    private Transform raycastOrigin; // Điểm xuất phát của tia raycast

    [SerializeField]
    private float rayLength = 10f; // Độ dài của tia raycast

    [SerializeField]
    private int damageAmount = 10; // Lượng sát thương gây ra cho người chơi

    [SerializeField]
    private float rotationSpeed = 90f; // Tốc độ quay (độ mỗi giây)

    [Networked(OnChanged = nameof(OnCurrentRotationChanged))]
    private Quaternion currentRotation { get; set; }

    private Animator animator;

    private bool canStateStart = false;

    // Dictionary để lưu trữ thời gian gây sát thương cuối cùng cho mỗi người chơi
   // private Dictionary<PlayerController, float> damagedPlayers = new Dictionary<PlayerController, float>();
    private static void OnCurrentRotationChanged(Changed<FlameBreathSkill> changed)
    {
        changed.Behaviour.UpdateParentRotation();
    }

    private void UpdateParentRotation()
    {
        transform.parent.rotation = currentRotation;
    }
    void Awake()
    {
        animator = GetComponentInParent<Animator>();

        // Đảm bảo object con (tia laser) ban đầu bị tắt
        if (flameBreathObject != null)
        {
            flameBreathObject.SetActive(false);
        }
        _dragonRoar.pitch = 0.9f;
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
            

            RPC_StartFlameBreath();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_StartFlameBreath()
    {
        if (flameBreathObject != null)
        {
            flameBreathObject.SetActive(true);
            animator.SetTrigger("Skill3");
            canStateStart = true;
            _dragonRoar.Play();
        }
    }

    public void FixedUpdateSkill()
    {
        if (Object.HasStateAuthority)
        {
            if (IsCasting && castingTimer.Expired(Runner))
            {
                RPC_EndFlameBreath();
                isCastingNetworked = false;
                

                OnSkillEnd?.Invoke();
            }

            if (isCastingNetworked && canStateStart)
            {
                // Tính toán góc quay mới dựa trên tốc độ và thời gian giữa các frame
                float rotationStep = rotationSpeed * Runner.DeltaTime;
                Quaternion deltaRotation = Quaternion.Euler(0f, rotationStep, 0f);

                // Cập nhật góc quay
                currentRotation *= deltaRotation;

                // Áp dụng góc quay mới cho object
                //transform.parent.rotation = currentRotation;
                PerformRaycast_RPC();
            }
        }   
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void PerformRaycast_RPC()
    {
        // Thực hiện raycast để kiểm tra va chạm với người chơi
        PerformRaycast();
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_EndFlameBreath()
    {
        if (flameBreathObject != null)
        {
            animator.SetTrigger("Idle");
            flameBreathObject.SetActive(false);
            canStateStart = false;
        }
    }

    void PerformRaycast()
    {
        if (raycastOrigin == null)
            return;

        // Tạo tia ray từ điểm xuất phát theo hướng forward của nó
        Ray ray = new Ray(raycastOrigin.position, raycastOrigin.forward);
        RaycastHit hit;

        // Thực hiện raycast
        if (Runner.GetPhysicsScene().Raycast(ray.origin, ray.direction, out hit, rayLength))
        {
            // Kiểm tra xem đối tượng va chạm có phải là người chơi không
            if (hit.collider != null && hit.collider.gameObject != null)
            {
                if (hit.collider.gameObject.TryGetComponent<PlayerController>(out var player))
                {
                    if(canTakeDamage)
                    {
                        // Gây sát thương cho người chơi
                        ApplyDamageToPlayer(player);
                    }
                    
                }
            }
        }
    }
    bool canTakeDamage = true;
    IEnumerator TakeDamageCountTime()
    {
        yield return new WaitForSeconds(1);
        canTakeDamage = true;
    }
    void ApplyDamageToPlayer(PlayerController player)
    {
        if (player != null)
        {
            player.TakeDamage_Boss(damageAmount);
            canTakeDamage = false;
            StartCoroutine(TakeDamageCountTime());
        }
    }
}
