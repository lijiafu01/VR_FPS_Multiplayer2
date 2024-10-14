using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;
using multiplayerMode;

public class FriendInvitationReceiver : MonoBehaviour
{
  
    [SerializeField]
    private GameObject _invitationTab;

    private bool isCheckingInvitations = false;

    [SerializeField]
    private TextMeshProUGUI _senderNameText;

    [SerializeField]
    private Button _acceptButton;

    [SerializeField]
    private Button _rejectButton;

    private Invitation currentInvitation;
    string _sessionName = null;

    void Start()
    {
        _invitationTab.SetActive(false);
        // Bắt đầu kiểm tra lời mời mỗi 3 giây
        StartCoroutine(CheckInvitationsRoutine());
    }

    IEnumerator CheckInvitationsRoutine()
    {
        isCheckingInvitations = true;
        while (isCheckingInvitations)
        {
            CheckForInvitations();
            yield return new WaitForSeconds(3f);
        }
    }

    public void CheckForInvitations()
    {
        var request = new GetUserDataRequest
        {
            PlayFabId = PlayFabSettings.staticPlayer.PlayFabId,
            Keys = new List<string> { "Invitations" }
        };
        PlayFabClientAPI.GetUserData(request, OnGetUserDataSuccess, OnGetUserDataFailure);
    }

    void OnGetUserDataSuccess(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("Invitations"))
        {
            string invitationsJson = result.Data["Invitations"].Value;
            List<Invitation> invitations = JsonUtility.FromJson<InvitationList>(invitationsJson).Invitations;

            if (invitations != null && invitations.Count > 0)
            {
                // Sắp xếp lời mời theo thời gian (mới nhất trước)
                invitations.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));

                // Lấy lời mời mới nhất
                Invitation newestInvitation = invitations[0];

                // Lấy thời gian hiện tại (Unix Time Milliseconds)
                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                // Kiểm tra xem lời mời có cũ hơn 5 giây không
                if (currentTime - newestInvitation.Timestamp <= 5000)
                {
                    // Hiển thị lời mời
                    ShowInvitationUI(newestInvitation);
                }
                else
                {
                    Debug.Log("Invitation is older than 5 seconds and will be ignored.");
                }

                // Xóa tất cả lời mời khỏi cloud
                ClearInvitations();
            }
        }
    }
    void ShowInvitationUI(Invitation invitation)
    {
        // Dừng việc kiểm tra lời mời
        isCheckingInvitations = false;

        // Lưu trữ lời mời hiện tại
        currentInvitation = invitation;

        // Hiển thị giao diện lời mời
        _invitationTab.SetActive(true);

        // Hiển thị tên người mời
        if (_senderNameText != null)
        {
            _senderNameText.text = $"{invitation.SenderName} invites you to play Boss.";
            
        }
        if(invitation.SessionName != null)
        {
            _sessionName = invitation.SessionName;

        }
      
        // Thiết lập các nút (để hàm rỗng)
        _acceptButton.onClick.RemoveAllListeners();
        _acceptButton.onClick.AddListener(OnAcceptInvitation);

        _rejectButton.onClick.RemoveAllListeners();
        _rejectButton.onClick.AddListener(OnRejectInvitation);

        // Bắt đầu coroutine tự động đóng sau 5 giây
       // StartCoroutine(AutoCloseInvitationUI());
    }
    public async void OnAcceptInvitation()
    {
        NetworkManager.Instance.PlayerName = PlayFabManager.Instance.UserData.DisplayName;
        NetworkManager.Instance.RoomName = _sessionName;
        _invitationTab.SetActive(false);
        await NetworkManager.Instance.FriendInviteGame(_sessionName);
        //_sessionName = null;
        // Đóng giao diện lời mời
        //_invitationTab.SetActive(false);

        // Tiếp tục kiểm tra lời mời
        //isCheckingInvitations = true;
    }

    public void OnRejectInvitation()
    {
        // Hàm rỗng, bạn sẽ tự viết logic

        // Đóng giao diện lời mời
        _invitationTab.SetActive(false);

        // Tiếp tục kiểm tra lời mời
        isCheckingInvitations = true;
    }
    void autoAccept()
    {
        OnAcceptInvitation();
    }
    IEnumerator AutoCloseInvitationUI()
    {
        yield return new WaitForSeconds(4f);
        //autoAccept();
       /* yield return new WaitForSeconds(5f);

        // Đóng giao diện lời mời
        _invitationTab.SetActive(false);

        // Tiếp tục kiểm tra lời mời
        isCheckingInvitations = true;*/
    }
    void OnGetUserDataFailure(PlayFabError error)
    {
        Debug.LogError("Failed to get user data: " + error.GenerateErrorReport());
    }

    public void AcceptInvitation(Invitation invitation)
    {
        // Tham gia phòng bằng cách sử dụng sessionName từ lời mời
        JoinGame(invitation.SessionName);
    }

    void JoinGame(string sessionName)
    {
        // Triển khai code tham gia phòng tại đây
        Debug.Log("Joining game session: " + sessionName);
    }

    public void ClearInvitations()
    {
        var request = new UpdateUserDataRequest
        {
            KeysToRemove = new List<string> { "Invitations" }
        };
        PlayFabClientAPI.UpdateUserData(request, OnClearInvitationsSuccess, OnClearInvitationsFailure);
    }

    void OnClearInvitationsSuccess(UpdateUserDataResult result)
    {
        Debug.Log("Invitations cleared.");
    }

    void OnClearInvitationsFailure(PlayFabError error)
    {
        Debug.LogError("Failed to clear invitations: " + error.GenerateErrorReport());
    }

    void OnDestroy()
    {
        // Dừng việc kiểm tra lời mời khi đối tượng bị hủy
        isCheckingInvitations = false;
    }

    [System.Serializable]
    public class Invitation
    {
        public string SenderId;
        public string SenderName;
        public string SessionName;
        public long Timestamp;
    }

    [System.Serializable]
    public class InvitationList
    {
        public List<Invitation> Invitations;
    }
}
