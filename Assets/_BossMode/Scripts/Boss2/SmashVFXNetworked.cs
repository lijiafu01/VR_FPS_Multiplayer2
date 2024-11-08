using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using multiplayerMode;
public class SmashVFXNetworked : NetworkBehaviour
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
        //audioSource.time = 0.35f;
        audioSource.Play();
        // Khởi tạo TickTimer để hủy đối tượng sau 2 giây
        lifeTimer = TickTimer.CreateFromSeconds(Runner, 3f);
        // Gây sát thương ngay khi vụ nổ được sinh ra
       /* if(Object.HasInputAuthority)
        {
            ApplyDamage();

        }*/


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
    private List<string> playerNames = new List<string>();
    private void OnTriggerEnter(Collider other)
    {
        if(Object == null) { return; }
        if (Object.HasStateAuthority)
        {
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
