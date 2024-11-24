using System;
using UnityEngine;
using TMPro;

public class PlayFabClock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText; // Text TMP hiển thị thời gian
    private DateTime utcTime; // Thời gian UTC từ PlayFab
    private float timeAccumulator = 0f; // Bộ đếm thời gian để cập nhật

    private void OnEnable()
    {
        // Đảm bảo Text TMP được gán
        if (timeText == null)
        {
            Debug.LogError("TextMeshProUGUI chưa được gán trong Inspector.");
            return;
        }

        // Lấy thời gian từ PlayFab
        PlayFabManager.Instance.GetPlayFabTime(playFabTime =>
        {
            if (playFabTime != DateTime.MinValue)
            {
                utcTime = playFabTime;
                UpdateTimeText(); // Hiển thị thời gian ngay khi nhận được
            }
            else
            {
                Debug.LogError("Không thể lấy thời gian từ PlayFab.");
            }
        });
    }

    private void Update()
    {
        // Tích lũy thời gian để mô phỏng đồng hồ
        timeAccumulator += Time.deltaTime;

        if (timeAccumulator >= 1f)
        {
            // Cập nhật mỗi giây
            utcTime = utcTime.AddSeconds(1);
            UpdateTimeText();
            timeAccumulator = 0f;
        }
    }

    /// <summary>
    /// Cập nhật Text TMP với thời gian hiện tại
    /// </summary>
    private void UpdateTimeText()
    {
        if (timeText != null)
        {
            timeText.text = utcTime.ToString("HH:mm"); // Định dạng giờ:phút
        }
    }
}
