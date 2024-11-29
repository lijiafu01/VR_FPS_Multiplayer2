using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
namespace multiplayerMode
{
public class GroupManagerTest : MonoBehaviour
{
    public string emailToSearch = "aaaaaa@gmail.com";  // Đặt email mà bạn muốn tìm kiếm
    private void Start()
    {
        // Đăng nhập vào PlayFab bằng email và mật khẩu trước khi tìm kiếm
        LoginToPlayFab();
    }
    private void LoginToPlayFab()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailToSearch,  // Địa chỉ email để đăng nhập
            Password = "aaaaaa"  // Mật khẩu đăng nhập
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }
    private void OnLoginSuccess(LoginResult result)
    {
        // Sau khi đăng nhập thành công, tìm kiếm người dùng bằng email
        SearchFriendByEmail();
    }
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("dev_Đăng nhập thất bại: " + error.GenerateErrorReport());
    }
    public void SearchFriendByEmail()
    {
        var request = new GetAccountInfoRequest
        {
            Email = emailToSearch  // Tìm kiếm người dùng bằng email
        };

        PlayFabClientAPI.GetAccountInfo(request, OnSearchSuccess, OnSearchFailure);
    }
    private void OnSearchSuccess(GetAccountInfoResult result)
    {
        Debug.Log("dev_Tìm thấy người chơi: " + result.AccountInfo.Username);
    }
    private void OnSearchFailure(PlayFabError error)
    {
        Debug.LogError("dev_Tìm kiếm thất bại: " + error.GenerateErrorReport());
    }
    public void SendFriendRequest(string friendPlayFabId)
    {
        var request = new AddFriendRequest
        {
            FriendPlayFabId = friendPlayFabId
        };

        PlayFabClientAPI.AddFriend(request, OnFriendRequestSent, OnFriendRequestFailed);
    }
    private void OnFriendRequestSent(AddFriendResult result)
    {
        Debug.Log("dev_Lời mời kết bạn đã được gửi!");
    }
    private void OnFriendRequestFailed(PlayFabError error)
    {
        Debug.LogError("dev_Gửi lời mời kết bạn thất bại: " + error.GenerateErrorReport());
    }
}
}

