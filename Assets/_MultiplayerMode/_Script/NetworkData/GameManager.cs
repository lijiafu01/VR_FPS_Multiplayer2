using UnityEngine;
namespace multiplayerMode
{
    public class GameManager : MonoBehaviour
    {
        // public TMP_InputField PlayerNameInput;
        // Biến static để lưu thể hiện duy nhất của GameManager
        public static GameManager Instance { get; private set; }
        // Dictionary để lưu trữ thông tin người chơi
        public PlayerData PlayerData { get; set; }
        // Đảm bảo GameManager không bị hủy khi chuyển cảnh
        private void Awake()
        {
            // Kiểm tra nếu đã có một thể hiện của GameManager
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Hủy đối tượng nếu đã tồn tại một thể hiện khác
                return;
            }
            Instance = this; // Gán thể hiện hiện tại cho biến static
            DontDestroyOnLoad(gameObject); // Đảm bảo GameManager không bị hủy khi chuyển cảnh
        }
    }
}