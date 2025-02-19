﻿using Fusion;
using UnityEngine;
using multiplayerMode;
public class ExplosionNetworked : NetworkBehaviour
{
    [SerializeField]
    private float damageRadius = 5f;

    [SerializeField]
    private float damageAmount = 20f;


    private TickTimer lifeTimer;
    
    public override void Spawned()
    {
        // Khởi tạo TickTimer để hủy đối tượng sau 2 giây
        lifeTimer = TickTimer.CreateFromSeconds(Runner, 2f);
        // Gây sát thương ngay khi vụ nổ được sinh ra
        if (Object.HasStateAuthority)
        {
            ApplyDamage();
        }


    }
    public override void FixedUpdateNetwork()
    {
        // Kiểm tra nếu TickTimer đã hết hạn
        if (lifeTimer.Expired(Runner))
        {
            // Hủy đối tượng trên mạng
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
                    health.TakeDamage_Boss((int)damageAmount);
                }
            }
        }
    }
}
