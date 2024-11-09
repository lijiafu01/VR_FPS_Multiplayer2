using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using multiplayerMode;
using UnityEngine.SceneManagement;
namespace multiplayerMode
{
    public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
    {
        /*[Networked, Capacity(30)]
        public NetworkDictionary<_playerRef, string> PlayerNames => default;*/
        [SerializeField]
        private NetworkPrefabRef playerPrefab; // Prefab của người chơi, được dùng để tạo các nhân vật trong game
                                               // Dictionary lưu trữ các nhân vật người chơi đã được spawn, để hủy chúng khi người chơi ngắt kết nối
        /*[Networked, Capacity(20)]
        public NetworkDictionary<_playerRef, NetworkObject> _spawnedUsers => default;*/

        private Dictionary<PlayerRef, NetworkObject> _spawnedUsers = new Dictionary<PlayerRef, NetworkObject>();
        public NetworkObject _networkPlayerObject;
        void Start()
        {
            // Thêm callback để nhận sự kiện mạng từ NetworkRunner
            NetworkManager.Instance.Runner.AddCallbacks(this);
        }
        /*public void PrintSpawnedUsers()
        {
            Debug.Log("Contents of _spawnedUsers:");

            foreach (var kvp in _spawnedUsers)
            {
                _playerRef player = kvp.Key;
                NetworkObject networkObject = kvp.Value;
                Debug.Log($"dev2_PlayerRef: {player.PlayerId}, NetworkObject: {networkObject.name}");
            }

            Debug.Log("End of _spawnedUsers contents.");
        }*/
        #region INetworkRunnerCallbacks
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("dev6_check join a1");
            if (SceneManager.GetActiveScene().name == "Boss1" || SceneManager.GetActiveScene().name == "MainGame" || SceneManager.GetActiveScene().name == "Boss2"|| SceneManager.GetActiveScene().name == "Boss3")
            {
                //Debug.Log("dev6_1OnPlayerJoined so player: " + _spawnedUsers.Count);

                Debug.Log("dev6_a2OnPlayerJoined id: " + player);

                // Kiểm tra nếu người chơi đã tồn tại trong danh sách
                if (!_spawnedUsers.ContainsKey(player))
                {
                    Debug.Log("dev7_check1");
                    // Khi một người chơi mới tham gia
                    if (player == runner.LocalPlayer)
                    {
                        Debug.Log("dev7_check2");
                        // Xác định vị trí spawn ngẫu nhiên trong phạm vi nhất định
                        Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(1, 5), 0.5f, UnityEngine.Random.Range(1, 5));

                        // Tạo nhân vật người chơi từ prefab và lưu trữ trong dictionary
                        _networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
                        if (_networkPlayerObject != null)
                        {
                            NetworkManager.Instance.NetworkPlayerObject = _networkPlayerObject;
                            NetworkManager.Instance.PlayerController = _networkPlayerObject.GetComponentInChildren<PlayerController>();
                           
                        }
                        _spawnedUsers.Add(player, _networkPlayerObject);
                        Debug.Log("dev7_check3");
                        //Debug.Log($"dev2_OnPlayerJoined  {player} + {_spawnedUsers.Count}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Player {player.PlayerId} is already spawned.");
                }
                Debug.Log("dev7_check4");
                // Debug.Log("dev6_2OnPlayerJoined so player: "+ _spawnedUsers.Count);
            }

        }
        /*public void OnPlayerJoined(NetworkRunner runner, _playerRef player)
        {
            Debug.Log("dev_OnPlayerJoined");
            // Khi một người chơi mới tham gia
            if (player == runner.LocalPlayer)
            {
                // Xác định vị trí spawn ngẫu nhiên trong phạm vi nhất định
                Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(1, 5), 0.5f, UnityEngine.Random.Range(1, 5));

                // Tạo nhân vật người chơi từ prefab và lưu trữ trong dictionary
                NetworkObject _networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
                _spawnedUsers.Add(player, _networkPlayerObject);
            }
        }*/
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (SceneManager.GetActiveScene().name != "Boss1")
            {
                return;
            }
            Debug.Log($"dev10_1OnPlayerLeft id: {player.PlayerId}");
            /*Debug.Log("dev10_1OnPlayerLeft");
            // Đảm bảo rằng _networkPlayerObject không phải là null
            if (_networkPlayerObject == null)
            {
                Debug.LogError("NetworkPlayerObject is null.");
                return;
            }

            // Tìm và lấy PlayerController từ tất cả các đối tượng con của _networkPlayerObject
            var playerController = _networkPlayerObject.GetComponentInChildren<PlayerController>();

            // Kiểm tra nếu không tìm thấy PlayerController
            if (playerController == null)
            {
                Debug.LogError("PlayerController script not found on any child objects.");
                return;
            }
            // Duyệt qua từng phần tử trong _PlayerDict và in ra thông tin
            foreach (var entry in playerController._PlayerDict)
            {
                _playerRef playerRef = entry.Key;
                NetworkObject networkObject = entry.Value;
            
                // In thông tin của từng player
                Debug.Log($"_playerRef ID: {playerRef.PlayerId}, NetworkObject: {networkObject.name}");
            }
            // Kiểm tra xem player có tồn tại trong từ điển không
            if (playerController._PlayerDict.ContainsKey(player))
            {
                // Lấy đối tượng mạng và despawn nó
                NetworkObject networkObject = playerController._PlayerDict[player];
                runner.Despawn(networkObject);

                // Xóa player khỏi từ điển
                playerController._PlayerDict.Remove(player);

                Debug.Log($"dev9_1Player {player.PlayerId} has been despawned and removed from the dictionary.");
            }
            Debug.Log($"dev9_2Player {player.PlayerId}");*/
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

