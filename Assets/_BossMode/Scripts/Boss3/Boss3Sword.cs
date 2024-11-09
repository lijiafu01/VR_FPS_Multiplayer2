using Fusion;
using multiplayerMode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Boss3Sword : NetworkBehaviour
{
    [SerializeField] private float impactForce;
    [SerializeField] private GameObject _Skill1VFX;

    [SerializeField] private int damageAmount = 20;
    private BossNetworked BossNetworkedScript;
    private Boss3Skill1 Boss3Skill1Script;
    private TickTimer lifeTimer;
    //public Transform Target;
    private Rigidbody rb;

    [SerializeField]
    private float forceStrength = 10f; // Cường độ lực đẩy

    public void Init(BossNetworked bossNetworked,Boss3Skill1 boss3Skill1)
    {
        BossNetworkedScript = bossNetworked;
        Boss3Skill1Script = boss3Skill1;
    }
    private void Awake()
    {
        // Gán Rigidbody của đối tượng
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody không được gán.");
        }
    }
    public void FlyObject(Transform Target)
    {
        // Kiểm tra nếu có mục tiêu
        if (Target != null && rb != null)
        {
            // Tính toán hướng đến mục tiêu
            Vector3 directionToTarget = (Target.position - transform.position).normalized;

            // Áp dụng lực đẩy về phía mục tiêu
            rb.AddForce(directionToTarget * forceStrength, ForceMode.Impulse);

            // Giới hạn vận tốc để tránh đẩy đối tượng đi quá xa
            float maxVelocity = 10f; // Tùy chỉnh giá trị vận tốc tối đa này
            if (rb.velocity.magnitude > maxVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }
        }
        else
        {
            Debug.LogError("Transform Null");
        }
    }
    public void FlyObject2(Transform Target, float rotationY)
    {
        if (Target != null && rb != null)
        {
            // Tính toán hướng đến mục tiêu
            Vector3 directionToTarget = (Target.position - transform.position).normalized;

            // Tính toán góc quay theo trục Y về phía mục tiêu và thêm rotationY
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            Quaternion adjustedRotation = Quaternion.Euler(lookRotation.eulerAngles.x, lookRotation.eulerAngles.y + rotationY, lookRotation.eulerAngles.z);

            // Áp dụng góc quay mới lên đối tượng
            transform.rotation = adjustedRotation;

            // Áp dụng lực đẩy về phía trước theo hướng đã xoay
            rb.AddForce(transform.forward * forceStrength, ForceMode.Impulse);

            // Giới hạn vận tốc để tránh đẩy đối tượng đi quá nhanh
            float maxVelocity = 10f; // Tùy chỉnh giá trị vận tốc tối đa này
            if (rb.velocity.magnitude > maxVelocity)
            {
                rb.velocity = rb.velocity.normalized * maxVelocity;
            }
        }
        else
        {
            Debug.LogError("Transform hoặc Rigidbody bị null");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.TryGetComponent<PlayerController>(out var health))
        {
            Boss3Skill1Script.OwnSword.SetActive(true);
            if (health != null)
            {
                health.TakeDamage_Boss(damageAmount);
                // Tính toán hướng từ BossNetworkedScript đến PlayerController
                Vector3 impactDirection = (other.transform.position - transform.position).normalized;

                // Gọi ReceiveImpact với hướng và cường độ lực
                health.ReceiveImpact(impactDirection, impactForce);

                Vector3 collisionPoint = other.ClosestPoint(transform.position);

                // Giữ nguyên giá trị Y là 0
                collisionPoint.y = 0;
                if (Object.HasStateAuthority)
                {
                    

                    // Đặt vị trí của BossNetworkedScript về điểm va chạm với Y cố định là 0
                    BossNetworkedScript.transform.position = collisionPoint;
                }
                // Instantiate một object tại một vị trí và xoay cụ thể
                Vector3 SpawVFXPos = new Vector3(collisionPoint.x, -1, collisionPoint.z);
                GameObject newObject = Instantiate(_Skill1VFX, SpawVFXPos, Quaternion.identity);
                Destroy( newObject,1.5f );
                // Hủy đối tượng sau khi xử lý
                Runner.Despawn(Object);
            }
        }
        

    }

    /*private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<PlayerController>(out var health))
        {
            if (health != null)
            {
                health.TakeDamage_Boss(damageAmount);
                Runner.Despawn(Object);
            }
            if (Object.HasStateAuthority)
            {
                Vector3 collisionPoint = other.contacts[0].point;
                BossNetworkedScript.transform.position = collisionPoint;
                
            }
        }
       
    }*/
    public override void Spawned()
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, 3f);

        
    }

    public override void FixedUpdateNetwork()
    {

        // Kiểm tra nếu TickTimer đã hết hạn
        if (lifeTimer.Expired(Runner) && Object.HasStateAuthority)
        {
            // Hủy đối tượng trên mạng
            Runner.Despawn(Object);
        }
    }

}
