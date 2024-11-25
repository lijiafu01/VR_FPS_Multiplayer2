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
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody không được gán.");
        }
    }
    public override void Spawned()
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, 3f);
        if (rb != null)
        {
            rb.AddForce(transform.forward * _Force, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("Rigidbody không được tìm thấy trên đối tượng.");
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (lifeTimer.Expired(Runner) && Object.HasStateAuthority)
        {
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
                health.ReceiveImpact(impactDirection, 6);
                health.TakeDamage_Boss(damageAmount);
            }
            if(Object.HasStateAuthority)
            {
                Runner.Despawn(Object);
            }
        }
    }
}
