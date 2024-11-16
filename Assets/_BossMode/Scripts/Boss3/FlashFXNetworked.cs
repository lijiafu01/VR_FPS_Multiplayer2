using Fusion;
using multiplayerMode;
using UnityEngine;

public class FlashFXNetworked : NetworkBehaviour
{
    [SerializeField] private float _Force = 20f;

    [SerializeField] private int damageAmount = 25;
    private TickTimer lifeTimer;
    private Rigidbody rb;
    private void Awake()
    {
        // Gán Rigidbody của đối tượng
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody không được gán.");
        }
    }
    
    public override void Spawned()
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, 3f);
        //rb = GetComponent<Rigidbody>();

        // Kiểm tra nếu Rigidbody đã được gán
        if (rb != null)
        {
            // Áp dụng lực đẩy về phía trước
            rb.AddForce(transform.forward * _Force, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("Rigidbody không được tìm thấy trên đối tượng.");
        }
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
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.TryGetComponent<PlayerController>(out var health))
        {
            if (health != null)
            {
                Vector3 impactDirection = (other.transform.position - transform.position).normalized;
                // Gọi ReceiveImpact với hướng và cường độ lực
                health.ReceiveImpact(impactDirection, 6);
                health.TakeDamage_Boss(damageAmount);
                // Tính toán hướng từ BossNetworkedScript đến PlayerController

            }
            if(Object.HasStateAuthority)
            {
                Runner.Despawn(Object);
            }
        }


    }
}
