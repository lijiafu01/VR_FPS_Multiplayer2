using Fusion;
using System.Collections;
using UnityEngine;
public class ThrowBallSkill : NetworkBehaviour, IBossSkill
{
    public AudioSource audioSource;
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
    [SerializeField]
    private Transform throwPoint;
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
            // Gọi RPC để đồng bộ hóa nếu cần
            RPC_SpawnFireball(firePoint.position, target.position);
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SpawnFireball(Vector3 spawnPosition, Vector3 targetPosition)
    {
        audioSource.Play();
        animator.SetTrigger("Skill1");
        StartCoroutine(SetAni(spawnPosition,targetPosition));    
    }
    IEnumerator SetAni(Vector3 spawnPosition, Vector3 targetPosition)
    {
        yield return new WaitForSeconds(0.3f); 
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
                isCastingNetworked = false;
                OnSkillEnd?.Invoke();
            }
        }
    }
    public void Explode(Vector3 collisionPosition, Vector3 collisionNormal)
    {
        Runner.Spawn(explosionPrefabNetworked, collisionPosition, Quaternion.LookRotation(collisionNormal), Object.InputAuthority);
    }
}
