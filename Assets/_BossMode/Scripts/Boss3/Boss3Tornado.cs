using Fusion;
using multiplayerMode;
using UnityEngine;
public class Boss3Tornado : NetworkBehaviour
{
    [SerializeField] private float impactForce = 2f;
    [SerializeField] private int damageAmount = 15;
    [SerializeField]
    private float speed = 5f; // Tốc độ di chuyển của Tornado
    private Vector3 direction; // Hướng di chuyển của Tornado
    private TickTimer lifeTimer;
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            // Chọn một hướng ngẫu nhiên trên mặt phẳng XZ
            float angle = Random.Range(0f, 360f);
            direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            direction = direction.normalized;
        }
        lifeTimer = TickTimer.CreateFromSeconds(Runner, 10f);
    }
    public override void FixedUpdateNetwork()
    {
        // Kiểm tra nếu TickTimer đã hết hạn
        if (lifeTimer.Expired(Runner))
        {
            if (Object.HasStateAuthority)
            {
                // Hủy đối tượng trên mạng
                Runner.Despawn(Object);
            }
            return;
        }
        if (Object.HasStateAuthority)
        {
            // Di chuyển đối tượng theo hướng đã chọn
            transform.position += direction * speed * Runner.DeltaTime;
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
                health.ReceiveImpact(impactDirection, impactForce);
                health.TakeDamage_Boss(damageAmount);
            }
        }
    }
}
