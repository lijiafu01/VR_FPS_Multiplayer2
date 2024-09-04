using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalManager : MonoBehaviour
{
    /*public GameObject MenuObject;  // Đối tượng menu
    private bool isMenuActive = false;  // Biến lưu trạng thái của menu*/
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
        // Ban đầu tắt menu
        //MenuObject.gameObject.SetActive(false);
        //test----------------
        //Invoke("LoadStartGameScene", 5f);
    }

   /* void Update()
    {
        // Kiểm tra nếu người chơi nhấn nút menu bên tay trái
        if (OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.LTouch))
        {
            Debug.Log("dev_Menu button pressed");
            OnMenuButtonPressed();
        }
    }

    // Hàm này sẽ được gọi khi nút menu bên tay trái được nhấn
    void OnMenuButtonPressed()
    {
        if (SceneManager.GetActiveScene().name == "MainGame")
        {
            // Đảo ngược trạng thái của menu (mở -> tắt, tắt -> mở)
            isMenuActive = !isMenuActive;

            // Thiết lập trạng thái menu dựa trên biến isMenuActive
            MenuObject.gameObject.SetActive(isMenuActive);
        }
    }*/
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
            Debug.Log("dev11_da xoa");

            yield return new WaitForSeconds(1f);
            Debug.Log("Shutting down Runner...");
            NetworkManager.Instance.Runner.Shutdown();
            // Chờ một chút để đảm bảo Shutdown được thực hiện
            yield return new WaitForSeconds(1.0f); // Bạn có thể điều chỉnh thời gian chờ
        }

        // Chuyển cảnh sau khi chắc chắn rằng Runner đã được shutdown
        Debug.Log("Loading StartGame scene...");
        SceneManager.LoadScene("StartGame");
    }

}
