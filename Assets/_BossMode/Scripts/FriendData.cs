using multiplayerMode;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class FriendData : MonoBehaviour
{
    public string FriendPlayFabId;
    public string FriendName;
    public TMP_Text playerNameText; // Tham chiếu đến Text UI hiển thị tên bạn bè
    public Button inviteButton; // Tham chiếu đến Button Invite
    private FriendInvitationSender _friendInvitationSender;
    public void Setup(FriendInfo friendInfo, FriendInvitationSender manager)
    {
        FriendPlayFabId = friendInfo.FriendPlayFabId;
        FriendName = friendInfo.TitleDisplayName;
        playerNameText.text = FriendName;
        _friendInvitationSender = manager;
        // Thêm listener cho nút Invite
        inviteButton.onClick.AddListener(OnInviteButtonClicked);
    }
    private void OnInviteButtonClicked()
    {
        // Gọi phương thức Invite trong FriendListManager
        _friendInvitationSender.InviteFriend(FriendPlayFabId);
    }
}
