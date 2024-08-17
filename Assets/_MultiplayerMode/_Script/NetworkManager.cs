using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Threading.Tasks;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    // Tạo một singleton để dễ dàng truy cập từ mọi nơi trong mã
    public static NetworkManager Instance { get; private set; }

    [SerializeField]
    private GameObject _runnerPrefab; // Prefab để tạo NetworkRunner

    public NetworkRunner Runner { get; private set; } // Runner dùng để quản lý mạng

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

    private void Start()
    {
        // Cố định máy chủ vào một khu vực cụ thể
        Fusion.Photon.Realtime.PhotonAppSettings.Instance.AppSettings.FixedRegion = "asia";
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
        // Thực hiện tương tự như CreateSession, nhưng dành cho người chơi tham gia vào một phiên đã có
        CreateRunner();
        await LoadScene();
        await Connect(roomCode);
    }

    public void CreateRunner()
    {
        // Tạo một NetworkRunner từ prefab và thêm callback để nhận sự kiện mạng
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

    #region INetworkRunnerCallbacks

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("A new player joined to the session"); // Log khi có người chơi mới tham gia
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("Runner Shutdown"); // Log khi runner dừng hoạt động
    }
    
    #endregion

    #region INetworkRunnerCallbacks (Unused)

    // Các phương thức callback khác được định nghĩa, nhưng chưa sử dụng. Có thể được mở rộng để xử lý sự kiện sau này
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
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
}
