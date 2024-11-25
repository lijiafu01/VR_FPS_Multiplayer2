using UnityEngine;
public interface IDamageable
{
    void TakeDamage(int damage, Vector3 hitPosition, Vector3 hitNormal, string shooterName,string teamID);
}
