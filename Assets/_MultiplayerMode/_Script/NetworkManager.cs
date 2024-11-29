using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Voice.Unity;
namespace multiplayerMode
{
    public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        public string BossName = null;
        public bool IsTeamMode = false;
        public string TeamID = null;
        private List<SessionInfo> _sessionList = new List<SessionInfo>();
        public NetworkObject NetworkPlayerObject;
        // Tạo một singleton để dễ dàng truy cập từ mọi nơi trong mã
        public static NetworkManager Instance { get; private set; }
        [SerializeField]
        public GameObject _runnerPrefab; // Prefab để tạo NetworkRunner
        public NetworkRunner Runner { get; private set; } // Runner dùng để quản lý mạng
        public PlayerSpawner PlayerSpawnerScript { get; private set; } // Script để tạo người chơi
        public PlayerRef _playerRef;
        public PlayerController PlayerController { get; set; }
        public Recorder RecorderScr { get; set; }
        public void UpdateTeamName(string newName)
        {
            TeamID = newName;
        }
        public int GenerateRandomSixDigitNumber()
        {
            // Số nguyên ngẫu nhiên từ 100000 đến 999999
            return UnityEngine.Random.Range(100000, 1000000);
        }
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
            TeamID = GenerateRandomSixDigitNumber().ToString();
        }
        private void Start()
        {
            // Cố định máy chủ vào một khu vực cụ thể
            Fusion.Photon.Realtime.PhotonAppSettings.Instance.AppSettings.FixedRegion = "asia";
            PlayerSpawnerScript = _runnerPrefab.GetComponent<PlayerSpawner>();
        }
        public void Init(Recorder recorder) 
        {
            RecorderScr = recorder;
        }
        public async void StartBossLobby()
        {
            CreateRunner();

            await Task.Delay(200);

            _networkEvents = Runner.gameObject.GetComponent<NetworkEvents>();
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
            IsTeamMode = false;
           // GameManager.Instance.PlayerData.playerName = GameManager.Instance.PlayerNameInput.text; // Tạo một PlayerData mới cho người chơi
            // Thực hiện tương tự như CreateSession, nhưng dành cho người chơi tham gia vào một phiên đã có
            CreateRunner();
            await LoadScene();
            await Connect(roomCode);
        }
        public async void JoinBossSession(string roomCode)
        {
            IsTeamMode = true;
            BossName = "Boss1";
            CreateRunner();
            await LoadSBosscene();
            await ConnectBoss(roomCode);
        }
        public async void JoinBoss2Session(string roomCode)
        {
            IsTeamMode = true;
            BossName = "Boss2";
            CreateRunner();
            await LoadSBoss2scene();
            await ConnectBoss2(roomCode);
        }
        public async void JoinBoss3Session(string roomCode)
        {
            IsTeamMode = true;
            BossName = "Boss3";
            CreateRunner();
            await LoadSBoss3scene();
            await ConnectBoss3(roomCode);
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
        public async Task LoadSBoss2scene()
        {
            // Tải scene không đồng bộ, chờ đến khi hoàn tất
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(4);
            while (!asyncLoad.isDone)
            {
                await Task.Yield();
            }
        }
        public async Task LoadSBoss3scene()
        {
            // Tải scene không đồng bộ, chờ đến khi hoàn tất
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(5);
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
        private async Task ConnectBoss2(string SessionName)
        {
            // Cấu hình và bắt đầu trò chơi với các tham số cần thiết
            var args = new StartGameArgs()
            {
                GameMode = GameMode.Shared, // Chế độ game chia sẻ
                SessionName = SessionName,  // Tên phiên
                SceneManager = GetComponent<NetworkSceneManagerDefault>(), // Quản lý scene mạng
                Scene = 4 // Scene đầu tiên (có thể là scene chính)
            };
            await Runner.StartGame(args);
        }
        private async Task ConnectBoss3(string SessionName)
        {
            // Cấu hình và bắt đầu trò chơi với các tham số cần thiết
            var args = new StartGameArgs()
            {
                GameMode = GameMode.Shared, // Chế độ game chia sẻ
                SessionName = SessionName,  // Tên phiên
                SceneManager = GetComponent<NetworkSceneManagerDefault>(), // Quản lý scene mạng
                Scene = 5 // Scene đầu tiên (có thể là scene chính)
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
            if (SceneManager.GetActiveScene().name == "Menu")
            {
                runner.Spawn(_playerNetworkDataPrefab, transform.position, Quaternion.identity, player);
            }
        }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if(SceneManager.GetActiveScene().name == "Menu")
            {
                if (_playerList.TryGetValue(player, out var playerNetworkData))
                {
                    runner.Despawn(playerNetworkData.Object);
                    _playerList.Remove(player);
                }
            }
        }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }
        #endregion
        #region INetworkRunnerCallbacks (Unused)
        // Các phương thức callback khác được định nghĩa, nhưng chưa sử dụng. Có thể được mở rộng để xử lý sự kiện sau này
        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
        }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner) {
        }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            _sessionList = sessionList;
        }
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
            // Kiểm tra xem phòng đã tồn tại hay chưa
            var session = _sessionList.Find(s => s.Name == RoomName);
            if (session == null)
            {
                var startGameArgs = new StartGameArgs()
                {
                    GameMode = GameMode.Host,
                    SessionName = RoomName,
                    SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                };
                var result = await Runner.StartGame(startGameArgs);
                if (result.Ok)
                {
                    BossLobbyManager menuManager = FindObjectOfType<BossLobbyManager>();
                    menuManager.SwitchMenuType(BossLobbyManager.MenuType.Room);
                    menuManager.SetStartBtnVisible(true);
                }
                else
                {
                    Debug.LogError($"Failed To Create Room: {result.ShutdownReason}");
                }
            }
            else
            {
                Debug.LogWarning("Room name already exists. Please choose a different name.");
            }
        }
        public async Task FriendInviteGame(string roomName)
        {
            StartBossLobby();
            await Task.Delay(3000);
            await JoinRoomFromFriendInvitation(roomName);
        }
        public async Task JoinRoomFromFriendInvitation(string roomName)
        {
            var startGameArgs = new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = roomName,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            };
            var result = await Runner.StartGame(startGameArgs);
            if (result.Ok)
            {
                BossLobbyManager menuManager = FindObjectOfType<BossLobbyManager>();
                menuManager.SwitchMenuType(BossLobbyManager.MenuType.Room);
                menuManager.SetStartBtnVisible(false);
            }
            else
            {
                Debug.LogError($"Failed To Join Room: {result.ShutdownReason}");
            }
        }
        public async Task JoinRoom()
        {
            // Kiểm tra xem phòng có tồn tại không
            var session = _sessionList.Find(s => s.Name == RoomName);
            if (session != null)
            {
                var startGameArgs = new StartGameArgs()
                {
                    GameMode = GameMode.Client,
                    SessionName = RoomName,
                    SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
                };
                var result = await Runner.StartGame(startGameArgs);
                if (result.Ok)
                {
                    BossLobbyManager menuManager = FindObjectOfType<BossLobbyManager>();
                    menuManager.SwitchMenuType(BossLobbyManager.MenuType.Room);
                    menuManager.SetStartBtnVisible(true);
                }
                else
                {
                    Debug.LogError($"Failed To Join Room: {result.ShutdownReason}");
                }
            }
            else
            {
                Debug.LogWarning("Room not found. Please check the room name.");
            }
        }
        public void StartGame()
        {
            int randomNumber = UnityEngine.Random.Range(10000, 100000); // Random số từ 10000 đến 99999
            // Chuyển số thành chuỗi
            string randomNumberString = randomNumber.ToString();
            if (_playerList.TryGetValue(_playerRef, out var playerNetworkData))
            {
                playerNetworkData.StartGame_RPC(randomNumberString);
            }
        }
        public void MidFuntion_StartBossScene()
        {
            StartCoroutine(StartBossScene());
        }
        public IEnumerator StartBossScene()
        {
            yield return new WaitForSeconds(1);
            if (_playerList.TryGetValue(_playerRef, out var playerNetworkData))
            {
                Runner.Despawn(playerNetworkData.Object);
                _playerList.Clear();
            }
            yield return new WaitForSeconds(1);
            Runner.Shutdown();
            yield return new WaitForSeconds(1);
            Runner = null;
            SetPlayerName();
            if(BossName == "Boss1")
            {
                JoinBossSession("BOSS");
            }
            if (BossName == "Boss2")
            {
                JoinBoss2Session("BOSS2");
            }
            if (BossName == "Boss3")
            {
                JoinBoss3Session("BOSS3");
            }
        }
        public void SetPlayerName()
        {
            // Kiểm tra nếu PlayerData chưa được khởi tạo
            if (GameManager.Instance.PlayerData == null)
            {
                GameManager.Instance.PlayerData = new PlayerData();
            }
            GameManager.Instance.PlayerData.playerName = PlayFabManager.Instance.UserData.DisplayName;
        }
        public void DestroyBoss_MidFuntion(string shooterName, string teamID)
        {
            StartCoroutine(DestroyBoss(shooterName, teamID));
        }
        IEnumerator DestroyBoss(string shooterName, string teamID)
        {
            yield return new WaitForSeconds(10f);
            if (teamID == TeamID)
            {
                PlayerController.KillBossReward();
            }
            else
            {
                PlayerController.ExitBoss();
            }
        }
    }
}

