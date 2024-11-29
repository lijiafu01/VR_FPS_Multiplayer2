using multiplayerMode;
using PlayFab.ClientModels;
using PlayFab;
using System;
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
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Update()
    {
    }
    /// <summary>
    /// Lấy thời gian hiện tại từ PlayFab
    /// </summary>
    /// <param name="onTimeReceived">Callback nhận thời gian</param>
    public void GetPlayFabTime(Action<DateTime> onTimeReceived)
    {
        PlayFabClientAPI.GetTime(new GetTimeRequest(), result =>
        {
            onTimeReceived?.Invoke(result.Time); // Trả về thời gian qua callback
        },
        error =>
        {
            Debug.LogError($"dev2_Lỗi khi lấy thời gian từ PlayFab: {error.GenerateErrorReport()}");
            onTimeReceived?.Invoke(DateTime.MinValue); // Trả về giá trị mặc định nếu có lỗi
        });
    }
}
