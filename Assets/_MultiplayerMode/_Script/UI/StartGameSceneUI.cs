using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using multiplayerMode;
using UnityEngine.SceneManagement;
namespace multiplayerMode
{
    public class StartGameSceneUI : MonoBehaviour
    {
        public TextMeshProUGUI AvatarNameText;
        public GameObject[] componentCanvas;
        public GameObject[] menuComponents;
        public GameObject MenuPanel;
        public TMP_InputField PlayerNameInput;
        public Button _button;

        private void Start()
        {
            AvatarNameText.text = PlayFabManager.Instance.UserData.DisplayName;
           // SceneManager.LoadScene("NewWeapon");
            //JoinRoom();
           // JoinBoss3Room();

            ///JoinBossRoom();
        }
        public void NextTraningSettingRoomScene()
        {
            SceneManager.LoadScene("TraningSettingRoom");
        }
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
                GameManager.Instance.PlayerData.playerName = PlayFabManager.Instance.UserData.DisplayName;
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
        public void CreateRoom1()
        {
            NetworkManager.Instance.BossName = "Boss1";
            NetworkManager.Instance.StartBossLobby();
        }
        public void CreateRoom2()
        {
            NetworkManager.Instance.BossName = "Boss2";

            NetworkManager.Instance.StartBossLobby();
        }
        public void CreateRoom3()
        {
            NetworkManager.Instance.BossName = "Boss3";

            NetworkManager.Instance.StartBossLobby();
        }


        public void JoinRoom()
        {
            SetPlayerName();
            NetworkManager.Instance.JoinSession("a");
        }
       /* private void OnEnable()
        {
           *//* if (PlayFabManager.Instance.isPC)
            {
                PlayerNameInput.text = "PC";
            }
            else
            {
                PlayerNameInput.text = "MetaQuestVR";
            }*//*
            JoinBossRoom();
        }*/
        public void JoinBossRoom()
        {
            SetPlayerName();
            NetworkManager.Instance.JoinBossSession("BOSS");
        }
        public void JoinBoss2Room()
        {
            SetPlayerName();
            NetworkManager.Instance.JoinBoss2Session("BOSS2");
        }
        public void JoinBoss3Room()
        {
            SetPlayerName();
            NetworkManager.Instance.JoinBoss3Session("BOSS3");
        }
        private void Update()
        {
            if (OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.LTouch))
            {
                foreach (GameObject MC in menuComponents)
                {
                    MC.SetActive(false);
                }
                foreach (GameObject CC in componentCanvas)
                {
                    CC.SetActive(true);
                }
            }
        }
    }
}

