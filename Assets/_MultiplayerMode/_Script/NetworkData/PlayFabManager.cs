using multiplayerMode;
using PlayFab.ClientModels;
using PlayFab;
using System;
using UnityEngine;
using System.Collections.Generic;
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

    //Sound setting------------------------------------

    public void LoadMicOnOff(Action<bool> callback)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            (result) => {
                bool micOnOff = false;
                if (result.Data != null && result.Data.ContainsKey("MicOnOff"))
                {
                    micOnOff = (result.Data["MicOnOff"].Value == "1");
                }
                // Gọi callback khi có dữ liệu
                callback?.Invoke(micOnOff);
            },
            (error) => {
                Debug.LogError("Failed to get user data: " + error.GenerateErrorReport());
                // Trong trường hợp lỗi, có thể trả về false hoặc giá trị mặc định
                callback?.Invoke(false);
            }
        );
    }

    // Hàm lấy dữ liệu speakerOnOff từ PlayFab và trả về qua callback
    public void LoadSpeakerOnOff(Action<bool> callback)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            (result) => {
                bool speakerOnOff = false;
                if (result.Data != null && result.Data.ContainsKey("SpeakerOnOff"))
                {
                    speakerOnOff = (result.Data["SpeakerOnOff"].Value == "1");
                }
                callback?.Invoke(speakerOnOff);
            },
            (error) => {
                Debug.LogError("Failed to get user data: " + error.GenerateErrorReport());
                callback?.Invoke(false);
            }
        );
    }

    // Hàm lấy dữ liệu volume từ PlayFab và trả về qua callback
    // Lần này là float
    public void LoadVolume(Action<float> callback)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            (result) => {
                float volume = 1.0f;
                if (result.Data != null && result.Data.ContainsKey("Volume"))
                {
                    float.TryParse(result.Data["Volume"].Value, out volume);
                }
                callback?.Invoke(volume);
            },
            (error) => {
                Debug.LogError("Failed to get user data: " + error.GenerateErrorReport());
                callback?.Invoke(1.0f); // Trả về giá trị mặc định trong trường hợp lỗi
            }
        );
    }

    // Các hàm save dữ liệu (ví dụ lưu micOnOff)
    public void SaveMicOnOff(bool micOnOff)
    {
        string micValue = micOnOff ? "1" : "0";
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"MicOnOff", micValue}
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnUserDataUpdateSuccess, OnUserDataUpdateFailure);
    }

    // Tương tự cho speakerOnOff
    public void SaveSpeakerOnOff(bool speakerOnOff)
    {
        string speakerValue = speakerOnOff ? "1" : "0";
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"SpeakerOnOff", speakerValue}
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnUserDataUpdateSuccess, OnUserDataUpdateFailure);
    }

    // Và cho volume
    public void SaveVolume(float volume)
    {
        string volumeValue = volume.ToString();
        var request = new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"Volume", volumeValue}
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnUserDataUpdateSuccess, OnUserDataUpdateFailure);
    }

    private void OnUserDataUpdateSuccess(UpdateUserDataResult result)
    {
        Debug.Log("User Data updated successfully!");
    }

    private void OnUserDataUpdateFailure(PlayFabError error)
    {
        Debug.LogError("Failed to update user data: " + error.GenerateErrorReport());
    }


}
