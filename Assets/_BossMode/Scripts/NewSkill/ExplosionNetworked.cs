using Fusion;
using UnityEngine;
using multiplayerMode;
public class ExplosionNetworked : NetworkBehaviour
{
    // Tham chiếu đến AudioSource
    public AudioSource audioSource;
    [SerializeField]
    private float damageRadius = 5f;
    [SerializeField]
    private int damageAmount = 20;
    private TickTimer lifeTimer;
    public override void Spawned()
    {
        audioSource.time = 0.35f;
        audioSource.Play();
        lifeTimer = TickTimer.CreateFromSeconds(Runner, 2f);
        ApplyDamage();
    }
    public override void FixedUpdateNetwork()
    {
        if (lifeTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }
    void ApplyDamage()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (var hit in hits)
        {
            // Kiểm tra nếu đối tượng có thể nhận sát thương
            if (hit.CompareTag("Player"))
            {
                var health = hit.GetComponent<PlayerController>();
                if (health != null)
                {
                    health.TakeDamage_Boss(damageAmount);
                    return;
                }
            }
        }
    }
}
