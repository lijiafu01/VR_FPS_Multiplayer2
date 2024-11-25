using multiplayerMode;
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
            }
        }
    }
}
