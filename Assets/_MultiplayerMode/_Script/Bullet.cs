using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [Networked]
    private TickTimer life { get; set; }

    [SerializeField]
    private float bulletSpeed = 5f;

    public override void Spawned()
    {
        life = TickTimer.CreateFromSeconds(Runner, 5.0f);
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else
        {
            // Di chuy?n vi?n ??n v? ph?a tr??c m?t c?ch m??t m?
            transform.Translate(Vector3.forward * bulletSpeed * Runner.DeltaTime, Space.Self);
        }
    }

}