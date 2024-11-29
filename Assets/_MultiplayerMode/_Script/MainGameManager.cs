using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace multiplayerMode
{
public class MainGameManager : MonoBehaviour
{
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
            yield return new WaitForSeconds(1f);
            NetworkManager.Instance.Runner.Shutdown();
            // Chờ một chút để đảm bảo Shutdown được thực hiện
            yield return new WaitForSeconds(1.0f); // Bạn có thể điều chỉnh thời gian chờ
        }
        // Chuyển cảnh sau khi chắc chắn rằng Runner đã được shutdown
        SceneManager.LoadScene("StartGame");
    }
}
}
