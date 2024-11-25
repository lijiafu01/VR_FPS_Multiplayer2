using Fusion;
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
      BossNetworked.TakeDamage(damage,hitPosition,hitNormal,shooterName,teamID);
    }
}
