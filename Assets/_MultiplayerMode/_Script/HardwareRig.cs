using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using System.Linq;
using TMPro;
using System.Xml;
using multiplayerMode;
using UnityEngine.UI;
namespace multiplayerMode
{
    public class HardwareRig : MonoBehaviour, INetworkRunnerCallbacks
    {
        public Transform RightHandAnchor;
        public Transform LeftHandAnchor;
        [SerializeField]
        private Slider healthSlider;
        [SerializeField]
        private TextMeshProUGUI hpText;
        [SerializeField]
        private GameObject hpCanvas;
        // Các Transform đại diện cho vị trí và hướng của player và các phần tử khác
        public Transform playerTransform;
        public Transform headTransform;
        public Transform leftHandTransform;
        public Transform rightHandTransform;

        public GameObject Content;
        public GameObject Template;
        // Từ điển để lưu trữ tên người chơi và điểm số
        public Dictionary<string, int> _playerScores = new Dictionary<string, int>();
        private Rigidbody rb;
        private void Start()
        {
            
            NetworkManager.Instance.Runner.AddCallbacks(this);
        }
       
        public void Death(Vector3 spawnPoint)
        {
            rb.velocity = Vector3.zero;
            transform.position = spawnPoint;
        }
        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }
        public void ReceiveImpact2(Vector3 direction, float force)
        {
            // Đảm bảo đối tượng có Rigidbody trước khi nhận lực
            if (rb == null) return;
            rb.velocity = Vector3.zero;
            // Tính toán vector lực theo hướng và cường độ
            Vector3 impactForce = direction.normalized * force;

            // Áp dụng lực vào Rigidbody
            rb.AddForce(impactForce, ForceMode.Impulse);
        }

        public void ReceiveImpact(Vector3 direction, float force)
        {
            // Đảm bảo đối tượng có Rigidbody trước khi nhận lực
            if (rb == null) return;

            // Đặt y của hướng về 0 nếu bạn muốn lực chỉ theo trục x và z
            direction.y = 0;

            // Tính toán vector lực theo hướng và cường độ
            Vector3 impactForce = direction.normalized * force;

            // Áp dụng lực vào Rigidbody
            rb.AddForce(impactForce, ForceMode.Impulse);
        }
        public int GetPlayerScore(string playerName)
        {
            // Kiểm tra xem người chơi có tồn tại trong từ điển điểm số hay không
            if (_playerScores.ContainsKey(playerName))
            {
                // Nếu tồn tại, trả về điểm số của người chơi
                return _playerScores[playerName];
            }
            else
            {
                Debug.LogWarning($"Player {playerName} not found in the score list.");
                return 0; // Trả về 0 nếu người chơi không có trong từ điển
            }
        }

        public void UpdateHP(int hp,int maxHP)
        {
            if (healthSlider != null)
            {
                healthSlider.value = (float)hp / maxHP;
            }

            string displayHP = "HP : " + hp.ToString() + " / " + maxHP;
            hpText.text = displayHP;
        }

        public void RemovePlayerFromLeaderboard(string playerName)
        {
            // Kiểm tra xem người chơi có tồn tại trong từ điển hay không
            if (_playerScores.ContainsKey(playerName))
            {
                // Xóa người chơi khỏi từ điển
                _playerScores.Remove(playerName);

                // Cập nhật lại bảng xếp hạng sau khi xóa người chơi
                UpdateLeaderboard();

                Debug.Log($"Player {playerName} has been removed from the leaderboard.");
            }
            else
            {
                Debug.LogWarning($"Player {playerName} not found in the leaderboard.");
            }
        }

        // Hàm để thêm hoặc cập nhật người chơi trên bảng xếp hạng
        public void AddOrUpdatePlayerOnLeaderboard(string playerName)
        {
            // Thêm hoặc cập nhật điểm số của người chơi trong từ điển local
            if (_playerScores.ContainsKey(playerName))
            {
                _playerScores[playerName] += 1;  // Tăng điểm số của người chơi nếu đã tồn tại
            }
            else
            {
                _playerScores.Add(playerName, 0);  // Thêm người chơi mới với điểm số 0
            }

        

            // Cập nhật bảng xếp hạng
            UpdateLeaderboard();
        }


        // Hàm để cập nhật bảng xếp hạng
        public void UpdateLeaderboard()
        {
            // Xóa tất cả các mục hiện có trong Content
            foreach (Transform child in Content.transform)
            {
                Destroy(child.gameObject);
            }

            // Sắp xếp danh sách người chơi theo điểm số giảm dần, và nếu điểm số bằng nhau thì sắp xếp theo tên
            var sortedPlayers = _playerScores
                .OrderByDescending(p => p.Value) // Sắp xếp theo điểm số giảm dần
                .ThenBy(p => p.Key) // Nếu điểm số bằng nhau, sắp xếp theo tên người chơi
                .ToList();

            int rank = 1;
            foreach (var player in sortedPlayers)
            {
                // Tạo một mục mới từ Template
                GameObject newEntry = Instantiate(Template, Content.transform);
                newEntry.SetActive(true);

                // Tìm TMP_Text trong đối tượng con của Template và cập nhật thông tin
                TMP_Text text = newEntry.GetComponentInChildren<TMP_Text>();
                if (text != null)
                {
                    text.text = $"{rank}. {player.Key} - {player.Value} points";
                }
                else
                {
                    Debug.LogError("TMP_Text component not found in template");
                }

                rank++;
            }
        }

        public void AddOrUpdatePlayerOnLeaderboardWithScore(string playerName, int playerScore)
        {
            // Thêm hoặc cập nhật điểm số của người chơi trong từ điển
            if (_playerScores.ContainsKey(playerName))
            {
                _playerScores[playerName] = playerScore;  // Cập nhật điểm số cụ thể cho người chơi
            }
            else
            {
                _playerScores.Add(playerName, playerScore);  // Thêm người chơi mới với điểm số cụ thể
            }

            // Cập nhật bảng xếp hạng
            UpdateLeaderboard();
        }

      


       /* void Start()
        {
            // Đăng ký HardwareRig để nhận callback từ NetworkRunner
            NetworkManager.Instance.Runner.AddCallbacks(this);
            //hpText = hpCanvas.transform.Find("HpText").GetComponent<TextMeshProUGUI>();

        }*/

        #region INetworkRunnerCallbacks

        void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
        {
            // Tạo một đối tượng RigState để chứa trạng thái hiện tại
            RigState xrRigState = new RigState();

            // Kiểm tra null cho từng phần tử trước khi truy cập thuộc tính của chúng
            if (headTransform != null)
            {
                xrRigState.HeadsetPosition = headTransform.position;
                xrRigState.HeadsetRotation = headTransform.rotation;
            }
            else
            {
                Debug.LogWarning("headTransform is null or has been destroyed.");
            }

            if (playerTransform != null)
            {
                xrRigState.PlayerPosition = playerTransform.position;
                xrRigState.PlayerRotation = playerTransform.rotation;
            }
            else
            {
                Debug.LogWarning("playerTransform is null or has been destroyed.");
            }

            if (leftHandTransform != null)
            {
                xrRigState.LeftHandPosition = leftHandTransform.position;
                xrRigState.LeftHandRotation = leftHandTransform.rotation;
            }
            else
            {
                Debug.LogWarning("leftHandTransform is null or has been destroyed.");
            }

            if (rightHandTransform != null)
            {
                xrRigState.RightHandPosition = rightHandTransform.position;
                xrRigState.RightHandRotation = rightHandTransform.rotation;
            }
            else
            {
                Debug.LogWarning("rightHandTransform is null or has been destroyed.");
            }

            xrRigState.ForceControllerLeft = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
            // Ghi nhận trạng thái vào NetworkInput để truyền qua mạng
            xrRigState.Button.Set(InputButton.Fire, OVRInputState.Instance.TriggerPressed);
            xrRigState.Button.Set(InputButton.Fire2, OVRInputState.Instance.LeftTriggerPressed);
            xrRigState.Button.Set(InputButton.Bow, OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch));

                //xrRigState.Button.Set(InputButton.Jump, OVRInputState.Instance.AButtonPressed);

            input.Set(xrRigState);
        }
          /*  bool isPulled = false;
         void Update()
         {
                if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
                {
                    isPulled = true;
                }
                else
                {
                    isPulled = false;
                }
         }*/
            #endregion

            #region Unused INetworkRunnerCallbacks

            // Các phương thức callback khác, chưa được sử dụng, có thể được mở rộng sau này
            void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
        void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        #endregion

    }

// Cấu trúc RigState lưu trữ trạng thái vị trí và hướng của các thành phần
public struct RigState : INetworkInput
{
    public Vector3 ForceControllerLeft;
    public NetworkButtons Button;
    public Vector3 PlayerPosition;
    public Quaternion PlayerRotation;

    public Vector3 HeadsetPosition;
    public Quaternion HeadsetRotation;

    public Vector3 LeftHandPosition;
    public Quaternion LeftHandRotation;

    public Vector3 RightHandPosition;
    public Quaternion RightHandRotation;
}

}
