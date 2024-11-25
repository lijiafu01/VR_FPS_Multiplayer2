using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
public class FriendInvitationSender : MonoBehaviour
{
    private List<FriendInfo> friendsList = new List<FriendInfo>();
    [SerializeField] private Transform _content;
    [SerializeField] private GameObject _friendTemplate;
    private BossLobbyManager _bossLobbyManager;
    private string playerDisplayName; // Lưu trữ DisplayName của người chơi
    private string friendID;
    private string USerID;
    private void OnEnable()
    {
        _bossLobbyManager = FindObjectOfType<BossLobbyManager>();
        playerDisplayName = PlayFabManager.Instance.UserData.DisplayName;
        USerID = PlayFabManager.Instance.UserData.UserID;
        GetFriends();
        Invoke("DisplayFriendList", 1);
    }
    void DisplayFriendList()
    {
        // Xóa các mục cũ trong danh sách
        foreach (Transform child in _content)
        {
            Destroy(child.gameObject);
        }
        // Tạo các mục mới cho danh sách bạn bè
        foreach (var friend in friendsList)
        {
            GameObject newFriendItem = Instantiate(_friendTemplate, _content);
            newFriendItem.SetActive(true);
            FriendData friendData = newFriendItem.GetComponent<FriendData>();
            friendData.Setup(friend, this);
        }
    }
    public void InviteFriend(string id)
    {
        SendInvitation(id,_bossLobbyManager.RoomName);
    }
    private void GetFriends()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(),
        result =>
        {
            if (result.Friends != null)
            {
                friendsList = result.Friends;
            }
        },
        error =>
        {
            Debug.LogError("Failed to get friends list: " + error.GenerateErrorReport());
        });
    }

    // Gửi lời mời cho bạn bè
    public void SendInvitation(string recipientPlayFabId, string sessionName)
    {
        if (string.IsNullOrEmpty(playerDisplayName))
        {
            return;
        }
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "SendInvitation",
            FunctionParameter = new
            {
                RecipientPlayFabId = recipientPlayFabId,
                SessionName = sessionName,
                SenderPlayFabId = PlayFabManager.Instance.UserData.UserID,
                SenderName = playerDisplayName // Truyền tên người gửi

            }
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnSendInvitationSuccess, OnSendInvitationFailure);
    }
    void OnSendInvitationSuccess(ExecuteCloudScriptResult result)
    {
        Debug.Log("Playfab_ Invitation sent successfully!");
    }
    void OnSendInvitationFailure(PlayFabError error)
    {
        Debug.LogError("Playfab_Failed to send invitation: " + error.GenerateErrorReport());
    }
}
