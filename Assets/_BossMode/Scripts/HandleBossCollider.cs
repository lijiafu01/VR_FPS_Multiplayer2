using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleBossCollider : NetworkBehaviour,IDamageable
{
    public BossNetworked BossNetworked;
    public void TakeDamage(int damage)
    {
      BossNetworked.TakeDamage(damage);
    }

   
}
