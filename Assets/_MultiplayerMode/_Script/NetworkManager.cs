using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
using multiplayerMode;
namespace multiplayerMode
{
    public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public NetworkObject NetworkPlayerObject;
        // Tạo một singleton để dễ dàng truy cập từ mọi nơi trong mã
        public static NetworkManager Instance { get; private set; }

        [SerializeField]
        public GameObject _runnerPrefab; // Prefab để tạo NetworkRunner

        public NetworkRunner Runner { get; private set; } // Runner dùng để quản lý mạng

        public PlayerSpawner PlayerSpawnerScript { get; private set; } // Script để tạo người chơi
        private void Awake()
        {
            // Đảm bảo chỉ có một thể hiện duy nhất của NetworkManager
            if (Instance != null && Instance != this)
            {             
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                
                DontDestroyOnLoad(this.gameObject); // Không hủy đối tượng khi tải scene mới
            }
        }

        private async void Start()
        {

            // Cố định máy chủ vào một khu vực cụ thể
            Fusion.Photon.Realtime.PhotonAppSettings.Instance.AppSettings.FixedRegion = "asia";
            PlayerSpawnerScript = _runnerPrefab.GetComponent<PlayerSpawner>();
            CreateRunner();
            Invoke("a", 3f);

        }
        async void a()
        {
            _networkEvents = Runner.gameObject.GetComponent<NetworkEvents>();
           /* _networkEvents.PlayerJoined.AddListener(OnPlayerJoined);
            _networkEvents.PlayerLeft.AddListener(OnPlayerLeft);*/
           
            //-------------
            var result = await Runner.JoinSessionLobby(SessionLobby.ClientServer);

            if (result.Ok)
            {
                SceneManager.LoadScene("Menu");
            }
            else
            {
                Debug.LogError($"Failed To Join Lobby: {result.ShutdownReason}");
            }
        }
        public async void CreateSession(string roomCode)
        {
            // Tạo Runner và bắt đầu phiên
            CreateRunner();
            // Tải scene cần thiết cho phiên
            await LoadScene();
            // Kết nối vào phiên với mã phòng
            await Connect(roomCode);
        }
        public async void JoinSession(string roomCode)
        {
           // GameManager.Instance.PlayerData.playerName = GameManager.Instance.PlayerNameInput.text; // Tạo một PlayerData mới cho người chơi
            // Thực hiện tương tự như CreateSession, nhưng dành cho người chơi tham gia vào một phiên đã có
            CreateRunner();
            await LoadScene();
            await Connect(roomCode);
        }
        public async void JoinBossSession(string roomCode)
        {
           // GameManager.Instance.PlayerData.playerName = GameManager.Instance.PlayerNameInput.text; // Tạo một PlayerData mới cho người chơi
            // Thực hiện tương tự như CreateSession, nhưng dành cho người chơi tham gia vào một phiên đã có
            CreateRunner();
            await LoadSBosscene();
            await ConnectBoss(roomCode);
        }
        public void CreateRunner()
        {
            // Kiểm tra nếu Runner đã tồn tại
            if (Runner != null)
            {
                Debug.LogWarning("dev_A NetworkRunner instance already exists. Reusing the existing runner.");
                return;
            }

            // Tạo một NetworkRunner từ prefab nếu chưa tồn tại
            Runner = Instantiate(_runnerPrefab, transform).GetComponent<NetworkRunner>();
            Runner.AddCallbacks(this);
        }
        public async Task LoadScene()
        {
            // Tải scene không đồng bộ, chờ đến khi hoàn tất
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2);

            while (!asyncLoad.isDone)
            {
                await Task.Yield();
            }
        }
        public async Task LoadSBosscene()
        {
            // Tải scene không đồng bộ, chờ đến khi hoàn tất
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(3);

            while (!asyncLoad.isDone)
            {
                await Task.Yield();
            }
        }

        private async Task Connect(string SessionName)
        {
            // Cấu hình và bắt đầu trò chơi với các tham số cần thiết
            var args = new StartGameArgs()
            {
                GameMode = GameMode.Shared, // Chế độ game chia sẻ
                SessionName = SessionName,  // Tên phiên
                SceneManager = GetComponent<NetworkSceneManagerDefault>(), // Quản lý scene mạng
                Scene = 2 // Scene đầu tiên (có thể là scene chính)
            };
            await Runner.StartGame(args);
        }
        private async Task ConnectBoss(string SessionName)
        {
            // Cấu hình và bắt đầu trò chơi với các tham số cần thiết
            var args = new StartGameArgs()
            {
                GameMode = GameMode.Shared, // Chế độ game chia sẻ
                SessionName = SessionName,  // Tên phiên
                SceneManager = GetComponent<NetworkSceneManagerDefault>(), // Quản lý scene mạng
                Scene = 3 // Scene đầu tiên (có thể là scene chính)
            };
            await Runner.StartGame(args);
        }

        #region INetworkRunnerCallbacks

        // Trong NetworkManager.cs
        public PlayerRef GetHostPlayerRef()
        {
            PlayerRef host = PlayerRef.None;

            foreach (var player in Runner.ActivePlayers)
            {
                if (host == PlayerRef.None || player < host)
                {
                    host = player;
                }
            }

            return host;
        }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            //boss lobby
            Debug.Log("runner1_playerNetworkDataPrefab");
            runner.Spawn(_playerNetworkDataPrefab, transform.position, Quaternion.identity, player);
            //---
        }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            //boss lobby
            if (_playerList.TryGetValue(player, out var playerNetworkData))
            {
                runner.Despawn(playerNetworkData.Object);
                _playerList.Remove(player);
            }
            //----
            Debug.Log("dev_player_left id:1111 "+player);
        }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("dev_OnShutdown11111");
            /*if (Runner != null)
            {
                Runner.Shutdown(); // Dừng hoạt động của Runner

                // Hủy bỏ đối tượng Runner sau khi shutdown hoàn tất
                Destroy(Runner.gameObject);

                // Đặt Runner về null để chắc chắn rằng nó không còn tồn tại
                Runner = null;

                Debug.Log("NetworkRunner has been shutdown and destroyed.");
            }
            Debug.Log("Runner Shutdown"); // Log khi runner dừng hoạt động*/
        }
    
        #endregion

        #region INetworkRunnerCallbacks (Unused)

        // Các phương thức callback khác được định nghĩa, nhưng chưa sử dụng. Có thể được mở rộng để xử lý sự kiện sau này
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {

        }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }

        #endregion

        //-----------------------
        public string PlayerName { get; set; }
        public string RoomName { get; set; }
        [SerializeField]
        private PlayerNetworkData _playerNetworkDataPrefab;


        [SerializeField]
        private NetworkEvents _networkEvents;

        public Dictionary<PlayerRef, PlayerNetworkData> PlayerList => _playerList;

        private Dictionary<PlayerRef, PlayerNetworkData> _playerList = new Dictionary<PlayerRef, PlayerNetworkData>();

        public void UpdatePlayerList()
        {
            var playerNames = new List<string>();
            foreach (var playerNetworkData in _playerList.Values)
            {
                playerNames.Add(playerNetworkData.PlayerName);
            }

            var menuManager = FindObjectOfType<BossLobbyManager>();
            menuManager.UpdatePlayerList(playerNames);
        }

        public void SetPlayerNetworkData(PlayerRef player, PlayerNetworkData playerNetworkData)
        {
            _playerList.Add(player, playerNetworkData);

            playerNetworkData.transform.SetParent(transform);
        }
       
        public async Task CreateRoom()
        {
            var result = await Runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Host,
                SessionName = RoomName,
                PlayerCount = 20,
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            });

            if (result.Ok)
            {
                var menuManager = FindObjectOfType<BossLobbyManager>();

                menuManager.SwitchMenuType(BossLobbyManager.MenuType.Room);
                menuManager.SetStartBtnVisible(true);

            }
            else
            {
                Debug.LogError($"Failed To Create Room: {result.ShutdownReason}");
            }
        }

        public async Task JoinRoom()
        {
            var result = await Runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = RoomName,
                PlayerCount = 20,
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            });

            if (result.Ok)
            {
                var menuManager = FindObjectOfType<BossLobbyManager>();

                menuManager.SwitchMenuType(BossLobbyManager.MenuType.Room);
                menuManager.SetStartBtnVisible(false);
            }
            else
            {
                Debug.LogError($"Failed To Join Room: {result.ShutdownReason}");
            }
        }
        public void StartGame()
        {
            //Runner.SetActiveScene("Boss1");
            Runner.Shutdown();
            Invoke("StartBossScene", 3f);
        }
        void StartBossScene()
        {
            SetPlayerName();
            NetworkManager.Instance.JoinBossSession("BOSS");
        }
        public void SetPlayerName()
        {
            // Kiểm tra nếu PlayerData chưa được khởi tạo
            if (multiplayerMode.GameManager.Instance.PlayerData == null)
            {
                multiplayerMode.GameManager.Instance.PlayerData = new PlayerData();
            }
            multiplayerMode.GameManager.Instance.PlayerData.playerName = "test";
        }
    }
}

