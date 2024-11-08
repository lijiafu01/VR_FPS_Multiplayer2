using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleBossCollider : NetworkBehaviour,IDamageable
{
    private BossNetworked BossNetworked;
    private void Start()
    {
        BossNetworked = GetComponentInParent<BossNetworked>();
    }
    public void TakeDamage(int damage,Vector3 hitPosition, Vector3 hitNormal, string shooterName, string teamID)
    {
        Debug.Log("boss1takedamage_boss 1");
      BossNetworked.TakeDamage(damage,hitPosition,hitNormal,shooterName,teamID);
    }
}
