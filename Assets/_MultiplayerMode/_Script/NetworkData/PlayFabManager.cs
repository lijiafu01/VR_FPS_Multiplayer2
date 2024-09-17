using multiplayerMode;
using UnityEngine;

public class PlayFabManager : MonoBehaviour
{
    // Biến tĩnh lưu trữ instance duy nhất của PlayFabManager
    public PlayFabCurrencyManager CurrencyManager;
    public UserData UserData;
    public static PlayFabManager Instance { get; private set; }

    // Được gọi khi script này được khởi tạo
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("dev2_Đã có một instance khác của PlayFabManager, hủy đối tượng mới.");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("dev2_Tạo instance duy nhất của PlayFabManager");
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    // Các hàm Start và Update có thể giữ nguyên nếu cần
    void Start()
    {
        // Code cần thực hiện khi game bắt đầu
    }

    void Update()
    {
        // Code cần thực hiện mỗi frame
    }
}
