using Fusion;
using multiplayerMode;
using UnityEngine;
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
            if (health != null)
            {
                health.TakeDamage_Boss(damageAmount);
                Vector3 impactDirection = (other.transform.position - transform.position).normalized;
                health.ReceiveImpact(impactDirection, impactForce);
                Vector3 collisionPoint = other.ClosestPoint(transform.position);
                collisionPoint.y = -1;
                if (Object.HasStateAuthority)
                {
                    BossNetworkedScript.transform.position = collisionPoint;
                }
                Vector3 SpawVFXPos = new Vector3(collisionPoint.x, -1, collisionPoint.z);
                GameObject newObject = Instantiate(_Skill1VFX, SpawVFXPos, Quaternion.identity);
                Destroy(newObject, 1.5f);
                Runner.Despawn(Object);
            }
            else
            {
                Debug.Log("checkclientGetdamege_ helth null");
            }
        }
    }
    public override void Spawned()
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, 3f);
    }
    public override void FixedUpdateNetwork()
    {
        if (lifeTimer.Expired(Runner) && Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }
}
