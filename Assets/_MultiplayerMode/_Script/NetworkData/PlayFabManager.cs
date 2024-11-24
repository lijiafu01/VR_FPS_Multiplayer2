using multiplayerMode;
using PlayFab.ClientModels;
using PlayFab;
using System;
using UnityEngine;

public class PlayFabManager : MonoBehaviour
{
    //public bool isPC =true;
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
   /* void Start()
    {
        if(Application.platform == RuntimePlatform.WindowsEditor)
        {
            isPC = true;
        }
        else
        {
            isPC = false;
        }
    }*/

    void Update()
    {
        // Code cần thực hiện mỗi frame
    }
    /// <summary>
    /// Lấy thời gian hiện tại từ PlayFab
    /// </summary>
    /// <param name="onTimeReceived">Callback nhận thời gian</param>
    public void GetPlayFabTime(Action<DateTime> onTimeReceived)
    {
        PlayFabClientAPI.GetTime(new GetTimeRequest(), result =>
        {
            Debug.Log($"dev2_Thời gian hiện tại trên PlayFab: {result.Time}");
            onTimeReceived?.Invoke(result.Time); // Trả về thời gian qua callback
        },
        error =>
        {
            Debug.LogError($"dev2_Lỗi khi lấy thời gian từ PlayFab: {error.GenerateErrorReport()}");
            onTimeReceived?.Invoke(DateTime.MinValue); // Trả về giá trị mặc định nếu có lỗi
        });
    }
}
