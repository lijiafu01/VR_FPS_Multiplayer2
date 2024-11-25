using Fusion;
using UnityEngine;
public class SummonMinionsSkill : NetworkBehaviour, IBossSkill
{
    [SerializeField]
    private GameObject _summonVFX;
    [SerializeField]
    private AudioSource _dragonAttack;
    public string SkillName => "Summon Minions";
    [SerializeField]
    private float cooldown = 20f;
    public float Cooldown => cooldown;
    [SerializeField]
    private float castingDuration = 2f; // Thời gian thi triển kỹ năng
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
    private NetworkPrefabRef minionPrefab; // Prefab của quái nhỏ
    [SerializeField]
    private int minionCount = 3; // Số lượng quái nhỏ sẽ triệu hồi
    [SerializeField]
    private float summonRadius = 5f; // Bán kính xung quanh Boss để sinh quái
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
            // Gọi RPC để đồng bộ hóa trên tất cả các client
            RPC_SummonMinions();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_SummonMinions()
    {
        _dragonAttack.Play();
        animator.SetTrigger("Skill4");
        // Triệu hồi quái nhỏ trên máy chủ
        if (Object.HasStateAuthority)
        {
            for (int i = 0; i < minionCount; i++)
            {
                Vector3 spawnPosition = GetRandomPositionAroundBoss();
                NetworkObject minion = Runner.Spawn(minionPrefab, spawnPosition, Quaternion.identity);
                GameObject summonVFX = Instantiate(_summonVFX, minion.gameObject.transform.position, minion.gameObject.transform.rotation);
                Destroy(summonVFX,1.5f );
            }
        }
    }
    Vector3 GetRandomPositionAroundBoss()
    {
        Vector2 randomCircle = Random.insideUnitCircle * summonRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 1f, randomCircle.y);
        return spawnPosition;
    }
    public void FixedUpdateSkill()
    {
        if (Object.HasStateAuthority)
        {
            if (IsCasting && castingTimer.Expired(Runner))
            {
                isCastingNetworked = false;
                OnSkillEnd?.Invoke();
            }
        }
    }
}
