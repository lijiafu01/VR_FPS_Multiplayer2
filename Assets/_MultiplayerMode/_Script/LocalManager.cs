using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace multiplayerMode
{
    public class LocalManager : MonoBehaviour
    {
         public GameObject LocalPlayer;
        // Biến tĩnh (static) giữ instance duy nhất của LocalManager
        private static LocalManager _instance;
        // Public property để truy cập instance từ bên ngoài
        public static LocalManager Instance
        {
            get
            {
                // Nếu instance chưa tồn tại, tìm nó trong scene
                if (_instance == null)
                {
                    _instance = FindObjectOfType<LocalManager>();
                    // Nếu không tìm thấy, có thể cảnh báo hoặc xử lý tùy ý
                    if (_instance == null)
                    {
                        Debug.LogError("No instance of LocalManager found in the scene.");
                    }
                }
                return _instance;
            }
        }
        // Đảm bảo rằng không có instance khác khi script này được khởi tạo
        private void Awake()
        {
            // Kiểm tra nếu đã tồn tại instance khác, hủy đối tượng này để đảm bảo chỉ có một instance
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        private void Start()
        {
        }
        public void LoadStartGameScene()
        {
            StartCoroutine(ShutdownAndLoadScene());
        }
        private IEnumerator ShutdownAndLoadScene()
        {
            // Kiểm tra nếu Runner tồn tại
            if (NetworkManager.Instance.Runner != null)
            {
                //Debug.Log($"dev11_test {NetworkManager.Instance.NetworkPlayerObject.gameObject.name}");
                NetworkManager.Instance.Runner.Despawn(NetworkManager.Instance.NetworkPlayerObject);
                yield return new WaitForSeconds(0.5f);
                NetworkManager.Instance.Runner.Shutdown();
                // Chờ một chút để đảm bảo Shutdown được thực hiện
                yield return new WaitForSeconds(1.0f); // Bạn có thể điều chỉnh thời gian chờ
            }
            // Chuyển cảnh sau khi chắc chắn rằng Runner đã được shutdown
            SceneManager.LoadScene("StartGame");
        }
    }
}
