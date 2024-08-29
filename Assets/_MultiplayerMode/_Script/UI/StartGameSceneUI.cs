using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartGameSceneUI : MonoBehaviour
{
    
    public TMP_InputField PlayerNameInput;
    public Button _button;
    public void SetPlayerName()
    {
        // Kiểm tra nếu PlayerData chưa được khởi tạo
        if (GameManager.Instance.PlayerData == null)
        {
            GameManager.Instance.PlayerData = new PlayerData();
        }

        // Kiểm tra nếu PlayerNameInput không bị null
        if (PlayerNameInput != null)
        {
            GameManager.Instance.PlayerData.playerName = PlayerNameInput.text;
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
    public void JoinRoom()
    {

        SetPlayerName();
        NetworkManager.Instance.JoinSession("a");
    }
}
