using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBuffVFXNetworked : NetworkBehaviour
{
    private TickTimer lifeTimer;
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
