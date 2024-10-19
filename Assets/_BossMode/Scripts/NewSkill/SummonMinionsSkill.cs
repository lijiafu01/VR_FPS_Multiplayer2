using Fusion;
using UnityEngine;
using static TrainerUI;

public class SummonMinionsSkill : NetworkBehaviour, IBossSkill
{
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

            // Kích hoạt animation nếu cần
            //animator.SetTrigger("Summon");

            // Gọi RPC để đồng bộ hóa trên tất cả các client
            RPC_SummonMinions();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_SummonMinions()
    {
        // Triệu hồi quái nhỏ trên máy chủ
        if (Object.HasStateAuthority)
        {
            for (int i = 0; i < minionCount; i++)
            {
                Vector3 spawnPosition = GetRandomPositionAroundBoss();
                NetworkObject minion = Runner.Spawn(minionPrefab, spawnPosition, Quaternion.identity);
                //var minionScript = minion.GetComponent<Minion>();
                // Nếu quái nhỏ có script riêng, bạn có thể khởi tạo chúng ở đây
                // Ví dụ:
                // var minionScript = minion.GetComponent<Minion>();
                // minionScript.Initialize(...);
            }
        }
    }

    Vector3 GetRandomPositionAroundBoss()
    {
        Vector2 randomCircle = Random.insideUnitCircle * summonRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

        // Đảm bảo vị trí sinh quái nằm trên mặt đất (nếu cần)
        // Bạn có thể sử dụng Raycast hoặc đặt y-coordinate bằng vị trí mặt đất

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
