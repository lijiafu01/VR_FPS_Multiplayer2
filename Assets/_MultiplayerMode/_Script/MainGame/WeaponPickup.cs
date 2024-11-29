using Fusion;
namespace multiplayerMode
{
    public class WeaponPickup : NetworkBehaviour
    {
        public Weapon weapon;
        [Networked(OnChanged = nameof(OnStateChanged))]
        private bool IsActive { get; set; } = true;
        public float respawnTime = 10f;
        private void DespawnPickup()
        {
            IsActive = false; // Cập nhật trạng thái để đồng bộ với các client khác
            Invoke(nameof(RespawnPickup), respawnTime);
        }
        private void RespawnPickup()
        {
            IsActive = true; // Cập nhật trạng thái để đồng bộ với các client khác
        }
        private static void OnStateChanged(Changed<WeaponPickup> changed)
        {
            bool isActive = changed.Behaviour.IsActive;
            changed.Behaviour.gameObject.SetActive(isActive); // Bật/tắt cục HP dựa trên trạng thái đồng bộ
        }
    }
}

