using UnityEngine;
using Fusion;
namespace multiplayerMode
{
    public class HealthPickup : NetworkBehaviour
    {
        public AudioSource hpSFX;
        [SerializeField] private int health = 30;
        [Networked(OnChanged = nameof(OnStateChanged))]
        private bool IsActive { get; set; } = true;
        public float respawnTime = 10f;
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        void SetSFX_RPC()
        {
            hpSFX.transform.position = transform.position;
            hpSFX.Play();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (IsActive && other.CompareTag("Player"))
            {
                if(Object.HasStateAuthority)
                {
                    SetSFX_RPC();
                }
                PlayerController player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.IncreaseHealth(health); // Tăng HP cho người chơi
                    DespawnPickup(); // Ẩn cục HP và bắt đầu thời gian chờ để respawn
                }
            }
        }
        private void DespawnPickup()
        {
            IsActive = false; // Cập nhật trạng thái để đồng bộ với các client khác
            Invoke(nameof(RespawnPickup), respawnTime);
        }
        private void RespawnPickup()
        {
            IsActive = true; // Cập nhật trạng thái để đồng bộ với các client khác
        }
        private static void OnStateChanged(Changed<HealthPickup> changed)
        {
            bool isActive = changed.Behaviour.IsActive;
            changed.Behaviour.gameObject.SetActive(isActive); // Bật/tắt cục HP dựa trên trạng thái đồng bộ
        }
    }
}

