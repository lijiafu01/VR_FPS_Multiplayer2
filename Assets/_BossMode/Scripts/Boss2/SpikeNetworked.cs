using Fusion;
using multiplayerMode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeNetworked : NetworkBehaviour
{
    [SerializeField] private NetworkObject _spikeVFX;

    // Tham chiếu đến AudioSource
    public AudioSource audioSource;


    [SerializeField]
    private int damageAmount = 20;


    private TickTimer lifeTimer;

    public override void Spawned()
    {
        //audioSource.time = 0.35f;
        //audioSource.Play();
        lifeTimer = TickTimer.CreateFromSeconds(Runner, 10f);

    }
    public override void FixedUpdateNetwork()
    {
        // Kiểm tra nếu TickTimer đã hết hạn
        if (lifeTimer.Expired(Runner) && Object.HasStateAuthority)
        {
            Runner.Spawn(_spikeVFX, transform.position, Quaternion.identity);

            // Hủy đối tượng trên mạng
            Runner.Despawn(Object);
        }
    }
    private List<string> playerNames = new List<string>();

    private void OnTriggerEnter(Collider other)
    {
        if (Object == null) { return; }
        if (other.TryGetComponent<PlayerController>(out var health))
        {
            Debug.Log("boss2_ st");

            if (health != null)
            {
                if (!playerNames.Contains(health.playerName))
                {
                    Debug.Log("boss2_ gây sát thương cho " + health.playerName);
                    playerNames.Add(health.playerName);
                    health.TakeDamage_Boss(damageAmount);
                }
            }
        }

        if (other.gameObject.tag == "LocalPlayer")
        {
            Debug.Log("boss2_va cham vat ly");
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 hitPosition = other.ClosestPoint(transform.position);
                rb.AddExplosionForce(6, hitPosition, 15, 10, ForceMode.Impulse);

            }
        }
    }

}
