using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
using Photon.Voice.Unity;
namespace multiplayerMode
{
    public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField]
        private NetworkPrefabRef playerPrefab; // Prefab của người chơi, được dùng để tạo các nhân vật trong game
        private Dictionary<PlayerRef, NetworkObject> _spawnedUsers = new Dictionary<PlayerRef, NetworkObject>();
        public NetworkObject _networkPlayerObject;
        void Start()
        {
            // Thêm callback để nhận sự kiện mạng từ NetworkRunner
            NetworkManager.Instance.Runner.AddCallbacks(this);
        }
        #region INetworkRunnerCallbacks
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (SceneManager.GetActiveScene().name == "Boss1" || SceneManager.GetActiveScene().name == "MainGame" || SceneManager.GetActiveScene().name == "Boss2"|| SceneManager.GetActiveScene().name == "Boss3")
            {
                // Kiểm tra nếu người chơi đã tồn tại trong danh sách
                if (!_spawnedUsers.ContainsKey(player))
                {
                    // Khi một người chơi mới tham gia
                    if (player == runner.LocalPlayer)
                    {
                        // Xác định vị trí spawn ngẫu nhiên trong phạm vi nhất định
                        Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(1, 5), 0.5f, UnityEngine.Random.Range(1, 5));
                        // Tạo nhân vật người chơi từ prefab và lưu trữ trong dictionary
                        _networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
                        if (_networkPlayerObject != null)
                        {
                            NetworkManager.Instance.NetworkPlayerObject = _networkPlayerObject;
                            NetworkManager.Instance.PlayerController = _networkPlayerObject.GetComponentInChildren<PlayerController>();
                            NetworkManager.Instance.Init(transform.GetComponentInChildren<Recorder>());
                        }
                        _spawnedUsers.Add(player, _networkPlayerObject);
                    }
                }
                else
                {
                    Debug.LogWarning($"Player {player.PlayerId} is already spawned.");
                }
            }
        }
        
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (SceneManager.GetActiveScene().name != "Boss1")
            {
                return;
            }
        }

        #endregion

        #region Unsed INetworkRunnerCallbacks

        // Các phương thức callback khác được định nghĩa, nhưng chưa sử dụng
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnDisconnectedFromServer(NetworkRunner runner) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

        #endregion
    }
}

