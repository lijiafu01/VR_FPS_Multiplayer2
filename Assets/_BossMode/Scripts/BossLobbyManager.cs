using multiplayerMode;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BossLobbyManager : MonoBehaviour
{
    public string RoomName => _roomNameInputField.text;
    public enum MenuType
    {
        Menu,
        Room
    }
    [SerializeField]
    private GameObject _menuPanel;
    [SerializeField]
    private GameObject _roomPanel;
    private string _playerName;
    [SerializeField]
    private TMP_InputField _roomNameInputField;
    [SerializeField]
    private Button _createBtn;
    [SerializeField]
    private Button _joinBtn;
    [SerializeField]
    private Button _startBtn;
    [SerializeField]
    private GameObject _playerListContent;
    [SerializeField]
    private PlayerListCell _playerListCell;
    private List<PlayerListCell> _existingCells = new List<PlayerListCell>();
    public void UpdatePlayerList(List<string> playerNames)
    {
        foreach (var cell in _existingCells)
        {
            Destroy(cell.gameObject);
        }
        _existingCells.Clear();
        foreach (var playerName in playerNames)
        {
            var cell = Instantiate(_playerListCell, _playerListContent.transform);
            cell.gameObject.SetActive(true);
            cell.SetPlayerName(playerName);
            _existingCells.Add(cell);
        }
    }
    private void Start()
    {
        _createBtn.onClick.AddListener(OnCreateBtnClicked);
        _joinBtn.onClick.AddListener(OnJoinBtnClicked);
        _startBtn.onClick.AddListener(OnStartBtnClicked);
        _playerName = PlayFabManager.Instance.UserData.DisplayName;
    }
    private void OnDestroy()
    {
        _createBtn.onClick.RemoveAllListeners();
        _joinBtn.onClick.RemoveAllListeners();
        _startBtn.onClick.RemoveAllListeners();
    }
    private async void OnCreateBtnClicked()
    {
        NetworkManager.Instance.PlayerName = _playerName;
        NetworkManager.Instance.RoomName = _roomNameInputField.text;
        await NetworkManager.Instance.CreateRoom();
    }
    private async void OnJoinBtnClicked()
    {
        NetworkManager.Instance.PlayerName = _playerName;
        NetworkManager.Instance.RoomName = _roomNameInputField.text;
        await NetworkManager.Instance.JoinRoom();
    }
    private void OnStartBtnClicked()
    {
        NetworkManager.Instance.StartGame();
    }
    public void SetStartBtnVisible(bool isVisible)
    {
        _startBtn.gameObject.SetActive(isVisible);
    }
    public void SwitchMenuType(MenuType menuType)
    {
        switch (menuType)
        {
            case MenuType.Menu:
                _menuPanel.SetActive(true);
                _roomPanel.SetActive(false);
                break;
            case MenuType.Room:
                _menuPanel.SetActive(false);
                _roomPanel.SetActive(true);
                break;
            default:
                break;
        }
    }
}
