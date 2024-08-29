using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalManager : MonoBehaviour
{
    public GameObject MenuObject;  // Đối tượng menu
    private bool isMenuActive = false;  // Biến lưu trạng thái của menu

    private void Start()
    {
        // Ban đầu tắt menu
        MenuObject.gameObject.SetActive(false);
        //test----------------
        //Invoke("LoadStartGameScene", 5f);
    }

    void Update()
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
    }
   /* public void LoadStartGameScene()
    {
        StartCoroutine(ShutdownAndLoadScene());
    }
*/
    /*private IEnumerator ShutdownAndLoadScene()
    {
        // Kiểm tra nếu Runner tồn tại
        if (NetworkManager.Instance.Runner != null)
        {
            Debug.Log("Shutting down Runner...");
            NetworkManager.Instance.Runner.Shutdown();
            // Chờ một chút để đảm bảo Shutdown được thực hiện
            yield return new WaitForSeconds(1.0f); // Bạn có thể điều chỉnh thời gian chờ
        }

        // Chuyển cảnh sau khi chắc chắn rằng Runner đã được shutdown
        Debug.Log("Loading StartGame scene...");
        SceneManager.LoadScene("StartGame");
    }*/

    /*public void LoadStartGameScene()
    {
        // Kiểm tra nếu Runner tồn tại
        if (NetworkManager.Instance.Runner != null)
        {
            // NetworkManager.Instance.PlayerSpawnerScript.OnPlayerLeft(NetworkManager.Instance.Runner, GameManager.Instance.PlayerData.playerRef);
            //Debug.Log($"dev_localManager_{GameManager.Instance.PlayerData.playerRef.PlayerId} x {GameManager.Instance.PlayerData.playerRef}");
            // Gọi hàm để rời khỏi session hoặc tắt runner
            //NetworkManager.Instance.Runner.Shutdown();
            NetworkManager.Instance.Runner.Shutdown();
        }
        SceneManager.LoadScene("StartGame");
    }*/
}
