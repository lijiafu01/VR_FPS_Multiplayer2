using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using multiplayerMode;
public class RubyNetworked : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (Object.HasStateAuthority)
        {
            if (other.gameObject.TryGetComponent<PlayerController>(out var hitPlayerController))
            {
                hitPlayerController.RubyNum++;
                Runner.Despawn(Object);
            }
        }
    }
    private TickTimer lifeTimer;
    public override void Spawned()
    {       
        lifeTimer = TickTimer.CreateFromSeconds(Runner, 9f);
    }
    public override void FixedUpdateNetwork()
    {
        // Kiểm tra nếu TickTimer đã hết hạn
        if (lifeTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }
}
