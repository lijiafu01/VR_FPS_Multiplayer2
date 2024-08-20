using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    public TMP_InputField PlayerNameInput;
    // Biến static để lưu thể hiện duy nhất của GameManager
    public static GameManager Instance { get; private set; }
    // Dictionary để lưu trữ thông tin người chơi
    public PlayerData PlayerData {get;set;}
    // Đảm bảo GameManager không bị hủy khi chuyển cảnh
    public void SetPlayerName()
    {
        // Kiểm tra nếu PlayerData chưa được khởi tạo
        if (PlayerData == null)
        {
            PlayerData = new PlayerData();
        }

        // Kiểm tra nếu PlayerNameInput không bị null
        if (PlayerNameInput != null)
        {
            PlayerData.playerName = PlayerNameInput.text;
        }
        else
        {
            Debug.LogError("PlayerNameInput is not assigned in the Inspector");
        }
    }
    public void SetQuickName()
    {
        // Tạo 4 số ngẫu nhiên
        int randomNumbers = Random.Range(0, 10000); // Tạo số ngẫu nhiên từ 0 đến 9999
        // Định dạng số ngẫu nhiên để đảm bảo có 4 chữ số (ví dụ: 0001, 0456, 9999)
        string formattedNumber = randomNumbers.ToString("D4");
        // Gán tên người chơi với 4 số ngẫu nhiên phía sau
        PlayerNameInput.text = "Player" + formattedNumber;
    }
    private void Awake()
    {
        // Kiểm tra nếu đã có một thể hiện của GameManager
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Hủy đối tượng nếu đã tồn tại một thể hiện khác
            return;
        }
        Instance = this; // Gán thể hiện hiện tại cho biến static
        DontDestroyOnLoad(gameObject); // Đảm bảo GameManager không bị hủy khi chuyển cảnh
    }
}
