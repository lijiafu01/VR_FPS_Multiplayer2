using multiplayerMode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerController>(out var health))
        {
            if (health != null)
            {
                
                health.TakeDamage_Boss(99999);
                // Tính toán hướng từ BossNetworkedScript đến PlayerController

            }
           
        }


    }
}
