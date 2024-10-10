using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class FriendInvitationSender : MonoBehaviour
{
    public string playerDisplayName; // Lưu trữ DisplayName của người chơi
    public string friendID;
    public string USerID;
    private void Start()
    {
        Debug.Log("Playfab_aaa");
        GetFriends();
        playerDisplayName = PlayFabManager.Instance.UserData.DisplayName;
        USerID = PlayFabManager.Instance.UserData.UserID;
        Invoke("Invite", 3f);
        
    }
    void Invite()
    {
        Debug.Log("Playfab_information_MyID:" + USerID + " friendID:" + friendID + "myname" + playerDisplayName);

        SendInvitation(friendID, "TEST");

    }
    private void GetFriends()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(),
        result =>
        {
            foreach (var friend in result.Friends)
            {
                string friendPlayFabId = friend.FriendPlayFabId;
                string friendUsername = friend.Username; // Nếu có
                string friendDisplayName = friend.TitleDisplayName; // Nếu có
                friendID = friendPlayFabId;
                Debug.Log("Friend PlayFab ID: " + friendPlayFabId);
                // Bạn có thể lưu trữ hoặc hiển thị danh sách bạn bè để người chơi chọn
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
            Debug.LogError("Player DisplayName is not set.");
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
