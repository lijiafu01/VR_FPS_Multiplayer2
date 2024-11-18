using Fusion;
using multiplayerMode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace multiplayerMode
{
    public class CardProjectileNetworked : NetworkBehaviour
    {
        private int _damage;
        public GameObject hitVFX;
        private TickTimer lifeTimer;
        public void Init(int damage)
        {
            _damage = damage;
            Debug.Log($"Init : damage {damage}");
        }
        
        public override void Spawned()
        {
            lifeTimer = TickTimer.CreateFromSeconds(Runner, 5f);

        }
        public override void FixedUpdateNetwork()
        {
            if (lifeTimer.Expired(Runner) && Object.HasStateAuthority)
            {
                Runner.Despawn(Object);
            }     
        }
        private void OnTriggerEnter(Collider other)
        {
            if(Object.HasStateAuthority)
            {
                
                // Lấy vị trí va chạm (hit point)
                Vector3 hitPoint = other.ClosestPoint(transform.position);
                // Tính toán pháp tuyến (normal)
                Vector3 hitNormal = (transform.position - hitPoint).normalized;
                if (other.TryGetComponent<PlayerController>(out var health))
                {
                    string playerName = GameManager.Instance.PlayerData.playerName;
                    if (health.playerName == playerName) return;
                    if (health != null)
                    {
                        health.TakeDamage_RPC(_damage, hitPoint, hitNormal, playerName);
                    }
                    GameObject vfx = Instantiate(hitVFX, transform.position, Quaternion.identity);
                    Destroy(vfx, 1f);
                    Runner.Despawn(Object);


                }
                else if (other.gameObject.TryGetComponent<IDamageable>(out var hitBossNetworked))
                {
                    string playerName = GameManager.Instance.PlayerData.playerName;

                    hitBossNetworked.TakeDamage(_damage, hitPoint, hitNormal, playerName, NetworkManager.Instance.TeamID);
                    GameObject vfx = Instantiate(hitVFX, transform.position, Quaternion.identity);
                    Destroy(vfx, 1f);
                    Runner.Despawn(Object);
                }
                else if (other.gameObject.TryGetComponent<IEnvironmentInteractable>(out var hitIEnvironmentInteractable))
                {

                    hitIEnvironmentInteractable.OnHitByWeapon();
                    GameObject vfx = Instantiate(hitVFX, transform.position, Quaternion.identity);
                    Destroy(vfx, 1f);
                    Runner.Despawn(Object);
                }
               
            }
            /*if (other.gameObject.tag == "Ground")
            {
                Instantiate(hitVFX, transform.position, Quaternion.identity);

                Runner.Despawn(Object);
            }*/
           
        }

           
            
           
    }

    
}

