using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FriendInvitationReceiver : MonoBehaviour
{
    private bool isCheckingInvitations = false;

    void Start()
    {
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

            foreach (var invitation in invitations)
            {
                Debug.Log($"You have been invited by {invitation.SenderId} to join session: {invitation.SessionName}");
                // Hiển thị lời mời trên UI và cho phép người chơi chấp nhận hoặc từ chối

                // Ví dụ: Tự động chấp nhận lời mời
                //AcceptInvitation(invitation);
            }

            // Sau khi xử lý, xóa lời mời
            ClearInvitations();
        }
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
        public string SessionName;
        public long Timestamp;
    }

    [System.Serializable]
    public class InvitationList
    {
        public List<Invitation> Invitations;
    }
}
