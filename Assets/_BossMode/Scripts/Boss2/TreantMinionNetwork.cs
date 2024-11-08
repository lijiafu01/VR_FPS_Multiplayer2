using Fusion;
using multiplayerMode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Fusion.NetworkCharacterController;

public class TreantMinionNetwork : NetworkBehaviour, IDamageable
{
    [SerializeField] private NetworkObject _HitTreeVFX;
    [SerializeField]
    private BoxCollider _boxCollider;
    [SerializeField]
    private float forceStrength = 3f; // Tùy chỉnh cường độ lực đẩy
    [SerializeField]
    private float liveTime;
    [SerializeField]
    private int damageAmount = 5;
    [SerializeField]
    private Animator _Animator;
    [SerializeField]
    private float _moveSpeed = 5f;

    [SerializeField]
    private float detectionRadius = 100f; // Bán kính tìm kiếm
    [SerializeField]
    private float stopDistance = 2f; // Khoảng cách dừng lại

    [SerializeField]
    private LayerMask playerLayerMask; // LayerMask cho Layer của người chơi

    private Transform targetPlayer; // Người chơi mục tiêu
    private TickTimer lifeTimer;

    // Cờ để kiểm soát trạng thái hiện tại
    private bool isMoving = true;
    private bool canAttack = true;
    private bool isDie = false;
    private Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public override void Spawned()
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, liveTime);
    }
    void FixedUpdate()
    {
        // Cố định rotation Y để luôn hướng về người chơi
        if (targetPlayer != null)
        {
            Vector3 direction = (targetPlayer.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 0.1f));
        }

        // Giữ góc xoay cố định trên trục X và Z
        rb.rotation = Quaternion.Euler(0, rb.rotation.eulerAngles.y, 0);
    }

    public override void FixedUpdateNetwork()
    {
        if(!isDie)
        {
            // Kiểm tra nếu TickTimer đã hết hạn
            if (lifeTimer.Expired(Runner) && Object.HasStateAuthority)
            {
                // Hủy đối tượng trên mạng
                Runner.Despawn(Object);
            }
            if (Object.HasStateAuthority)
            {
                // Kiểm tra và tìm người chơi mục tiêu nếu chưa có
                if (targetPlayer == null)
                {
                    targetPlayer = FindRandomPlayer();
                }
                // Nếu có người chơi mục tiêu, di chuyển về phía người chơi đó
                if (targetPlayer != null)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, targetPlayer.position);

                    // Kiểm tra nếu khoảng cách lớn hơn stopDistance
                    if (distanceToTarget > stopDistance && isMoving)
                    {
                        // Di chuyển về phía người chơi mục tiêu
                        Vector3 direction = (targetPlayer.position - transform.position).normalized;
                        transform.position += direction * _moveSpeed * Runner.DeltaTime;

                        // Hướng boss về phía người chơi mục tiêu
                        Quaternion targetRotation = Quaternion.LookRotation(direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.1f);


                    }
                    else
                    {
                        isMoving = false;
                        // Kích hoạt trạng thái "Tấn công" nếu chưa ở trong trạng thái tấn công
                        if (!isMoving && canAttack)
                        {
                            if (targetPlayer != null)
                            {
                                Vector3 directionToTarget = (targetPlayer.position - transform.position).normalized;
                                transform.rotation = Quaternion.LookRotation(directionToTarget);
                            }
                            SetAnimator_RPC();
                            canAttack = false;
                            Invoke("Attacking", 2f);

                        }
                    }
                }
            }
        }
       
       
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void SetAnimator_RPC()
    {
        _boxCollider.enabled = true;
        _Animator.SetTrigger("Attack");

    }
    void Attacking()
    {
        canAttack = true;
        isMoving = true;
    }
   

   
    
    private void OnTriggerEnter(Collider other)
    {
        /*Debug.Log("checkquyenhan_HasInputAuthority_treanMinion : " + Object.HasInputAuthority + " " );
        Debug.Log("checkquyenhan_HasStateAuthority_treanMinion : " + Object.HasStateAuthority + " " );*/
        if (other.TryGetComponent<PlayerController>(out var health))
        {

            if (health != null)
            {
                health.TakeDamage_Boss(damageAmount);
                _boxCollider.enabled = false;
            }
        }


        if (other.CompareTag("LocalPlayer"))
        {
            Debug.Log("boss2_va cham vat ly");
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Tính vector hướng từ boss đến người chơi
                Vector3 directionToPlayer = (other.transform.position - transform.position).normalized;

                // Đặt y thành 0 để chỉ có lực tác động trên trục x và z (ngang)
                directionToPlayer.y = 0;

                // Áp dụng lực đẩy về phía sau với ForceMode.Impulse

                rb.AddForce(directionToPlayer * forceStrength, ForceMode.Impulse);

                //_boxCollider.enabled = false;

            }
        }

    }

    Transform FindRandomPlayer()
    {
        // Tìm tất cả các đối tượng trong bán kính phát hiện thuộc lớp người chơi
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayerMask);

        // Kiểm tra nếu có bất kỳ đối tượng nào được tìm thấy
        if (hits.Length > 0)
        {
            // Chọn một đối tượng ngẫu nhiên từ danh sách hits
            int randomIndex = Random.Range(0, hits.Length);
            return hits[randomIndex].transform;
        }

        // Nếu không tìm thấy đối tượng nào, trả về null
        return null;
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_TakeDamage(int damage, Vector3 hitPosition, Vector3 hitNormal, string shooterName, string teamID)
    {
        if (Object.HasStateAuthority && !isDie)
        {
            isDie = true;
            Runner.Spawn(_HitTreeVFX, hitPosition, Quaternion.LookRotation(hitNormal));
            SetAnimationDie_RPC();
            Invoke("DestroyObject", 1.2f);

        }
    }
    public void TakeDamage(int damage, Vector3 hitPosition, Vector3 hitNormal, string shooterName, string teamID)
    {
        // Gửi RPC tới máy có State Authority
        RPC_TakeDamage(damage, hitPosition, hitNormal, shooterName, teamID);
        
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void SetAnimationDie_RPC()
    {
        _Animator.SetTrigger("Die");

    }
    void DestroyObject()
    {
        Runner.Despawn(Object);
    }
}
