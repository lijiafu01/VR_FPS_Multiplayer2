using Fusion;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ThrowBallSkill : NetworkBehaviour, IBossSkill
{
    [SerializeField] private NetworkPrefabRef explosionPrefabNetworked;
    public GameObject fireballPrefab;

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
   /* [SerializeField]
    private NetworkPrefabRef ballPrefab;*/

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
            RPC_SpawnFireball(firePoint.position, target.position);
        }
    }
   
    

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SpawnFireball(Vector3 spawnPosition, Vector3 targetPosition)
    {
        animator.SetTrigger("Skill1");
        // Tạo Fireball trên client
        GameObject fireballObject = Instantiate(fireballPrefab, spawnPosition, Quaternion.identity);

        // Lấy script Fireball và thiết lập các biến cần thiết
        Fireball fireball = fireballObject.GetComponent<Fireball>();
        if (fireball != null)
        {
            fireball.Initialize(targetPosition);

        }

        if (Object.HasStateAuthority)
        {
            fireball.ThrowBallSkill = this;
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
    public void Explode(Vector3 collisionPosition, Vector3 collisionNormal)
    {
        Runner.Spawn(explosionPrefabNetworked, collisionPosition, Quaternion.LookRotation(collisionNormal), Object.InputAuthority);
    }
}
