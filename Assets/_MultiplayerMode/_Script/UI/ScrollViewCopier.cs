using UnityEngine;
using UnityEngine.SceneManagement;
public class ScrollViewCopier : MonoBehaviour
{
    private Transform contentA; // Content của ScrollView A
    [SerializeField] private Transform contentB; // Content của ScrollView B
    private Intermediary intermediaryScript; // Biến lưu script Intermediary
    bool isMainGame = false;
    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "MainGame")
        {
            isMainGame = true;
        }
        else
        {
            return;
        }
        // Tìm GameObject có tên "Intermediary"
        GameObject intermediaryObject = GameObject.Find("Intermediary");

        if (intermediaryObject != null)
        {
            // Lấy script Intermediary từ đối tượng
            intermediaryScript = intermediaryObject.GetComponent<Intermediary>();
            if (intermediaryScript != null)
            {
                contentA = intermediaryScript.RankContent;
            }
            else
            {
                Debug.LogError("Script Intermediary không được gắn trên GameObject 'Intermediary'.");
            }
        }
        else
        {
            Debug.LogError("GameObject 'Intermediary' không được tìm thấy trong Scene.");
        }
    }
    private void OnEnable()
    {
        if(!isMainGame) return;
        CopyContent();
    }
    public void CopyContent()
    {
        // Kiểm tra nếu cả hai content đã được gán
        if (contentA == null || contentB == null)
        {
            Debug.LogError("Content A hoặc Content B chưa được gán!");
            return;
        }
        // Xóa tất cả nội dung hiện tại trong Content B
        foreach (Transform child in contentB)
        {
            Destroy(child.gameObject);
        }
        // Sao chép nội dung từ Content A sang Content B
        foreach (Transform child in contentA)
        {
            GameObject newChild = Instantiate(child.gameObject, contentB);
            newChild.SetActive(true); // Đảm bảo đối tượng được kích hoạt
        }
    }
}
