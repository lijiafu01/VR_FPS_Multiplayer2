using Fusion;
using UnityEngine;

public class FlameBreathSkill : NetworkBehaviour, IBossSkill
{
    public string SkillName => "Flame Breath";

    [SerializeField]
    private float cooldown = 10f;
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
    private GameObject flameBreathPrefab;

    [SerializeField]
    private Transform flameSpawnPoint;

    private Animator animator;

    void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    public void ActivateSkill(Transform target)
    {
        if (Object.HasStateAuthority && !IsOnCooldown && !IsCasting)
        {
            isCastingNetworked = true;
            castingTimer = TickTimer.CreateFromSeconds(Runner, CastingDuration);
            cooldownTimer = TickTimer.CreateFromSeconds(Runner, Cooldown);

            OnSkillStart?.Invoke();

            animator.SetTrigger("Skill3");

            // Gọi RPC để kích hoạt hiệu ứng trên tất cả các client
            RPC_StartFlameBreath();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_StartFlameBreath()
    {
        // Kích hoạt hiệu ứng hạt
        GameObject flameEffect = Instantiate(flameBreathPrefab, flameSpawnPoint.position, flameSpawnPoint.rotation);
        flameEffect.transform.parent = flameSpawnPoint;

        // Tự động hủy hiệu ứng sau một thời gian
        Destroy(flameEffect, CastingDuration);
    }

    public void FixedUpdateSkill()
    {
        if (Object.HasStateAuthority && isCastingNetworked)
        {
            if (castingTimer.Expired(Runner))
            {
                isCastingNetworked = false;
                OnSkillEnd?.Invoke();
            }
        }
    }
}
