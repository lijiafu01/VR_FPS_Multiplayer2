using UnityEngine;
using multiplayerMode;
namespace multiplayerMode
{
    public class HealingItemManager : MonoBehaviour
    {
        public GameObject[] healthPickups; // Gán các cục HP hiện có trong scene

        void Start()
        {
            if (NetworkManager.Instance.Runner.IsServer)
            {
                // Chuyển đổi từng cục HP trong scene thành đối tượng mạng
                foreach (var pickup in healthPickups)
                {
                    Vector3 position = pickup.transform.position;
                    Quaternion rotation = pickup.transform.rotation;

                    // Spawn lại đối tượng qua Runner để nó được đồng bộ hóa
                    NetworkManager.Instance.Runner.Spawn(pickup, position, rotation);
                }
            }
        }
    }
}

