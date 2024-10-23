using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damage, Vector3 hitPosition, Vector3 hitNormal, string shooterName,string teamID);
}
