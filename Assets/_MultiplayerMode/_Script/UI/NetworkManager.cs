/*using Fusion;
using multiplayerMode;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
public class NetworkManager : MonoBehaviour
{
    public string PlayerName { get; set; }
    public string RoomName { get; set; }
    [SerializeField]
    private PlayerNetworkData _playerNetworkDataPrefab;

    public static NetworkManager Instance { get; private set; }

    [SerializeField]
    private NetworkRunner Runner;

    [SerializeField]
    private NetworkEvents _networkEvents;

    public Dictionary<_playerRef, PlayerNetworkData> PlayerList => _playerList;

    private Dictionary<_playerRef, PlayerNetworkData> _playerList = new Dictionary<_playerRef, PlayerNetworkData>();

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

    public void SetPlayerNetworkData(_playerRef player, PlayerNetworkData playerNetworkData)
    {
        _playerList.Add(player, playerNetworkData);

        playerNetworkData.transform.SetParent(transform);
    }
    private void OnPlayerJoined(NetworkRunner runner, _playerRef player)
    {
        runner.Spawn(_playerNetworkDataPrefab, transform.position, Quaternion.identity, player);
    }

    private void OnPlayerLeft(NetworkRunner runner, _playerRef player)
    {
        if (_playerList.TryGetValue(player, out var playerNetworkData))
        {
            runner.Despawn(playerNetworkData.Object);
            _playerList.Remove(player);
        }
    }



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            _networkEvents.PlayerJoined.AddListener(OnPlayerJoined);
            _networkEvents.PlayerLeft.AddListener(OnPlayerLeft);

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private async void Start()
    {
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
    public async Task CreateRoom()
    {
        var result = await Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
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
            GameMode = GameMode.Shared,
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
        Runner.SetActiveScene("Boss1");
        *//*Runner.Shutdown();
        Invoke("StartBossScene", 3f);*//*
    }
   *//* void StartBossScene()
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
    }*//*
}
*/