using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using multiplayerMode;
namespace multiplayerMode
{
public class MainGameManager : MonoBehaviour
{
    private void Start()
    {
        // Ban đầu tắt menu
        // MenuObject.gameObject.SetActive(false);
        //test----------------
        //Invoke("LoadStartGameScene", 7f);
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

}
