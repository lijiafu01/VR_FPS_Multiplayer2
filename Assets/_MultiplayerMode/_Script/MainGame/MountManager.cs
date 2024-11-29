using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
namespace multiplayerMode
{
    public class MountManager : NetworkBehaviour
    {
        public GameObject[] mountsInScene;  // Các đối tượng thú cưỡi đã được đặt sẵn trong scene
        private List<GameObject> activeMounts = new List<GameObject>(); // Danh sách quản lý thú cưỡi hoạt động
        private void Start()
        {
            // Thêm thú cưỡi có sẵn trong scene vào danh sách quản lý
            foreach (var mount in mountsInScene)
            {
                if (mount != null)
                {
                    activeMounts.Add(mount);
                }
            }
        }
        // Phương thức để bắt đầu cho thú cưỡi bay khi người chơi va chạm
        public void PlayerMount(GameObject player, GameObject mount)
        {
            // Kiểm tra nếu thú cưỡi đang hoạt động và chưa có người cưỡi
            if (activeMounts.Contains(mount))
            {
                Mount mountScript = mount.GetComponent<Mount>();
                if (mountScript != null)
                {
                    mountScript.StartFlying(player); // Bắt đầu cho thú cưỡi bay
                }
            }
        }
        // Phương thức để dừng thú cưỡi khi người chơi không còn cưỡi nữa
        public void PlayerDismount(GameObject mount)
        {
            if (activeMounts.Contains(mount))
            {
                Mount mountScript = mount.GetComponent<Mount>();
                if (mountScript != null)
                {
                    mountScript.StopFlying(); // Dừng thú cưỡi
                }
            }
        }
    }
}
