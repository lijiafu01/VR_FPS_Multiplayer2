using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Thư viện cho TextMeshPro
using UnityEngine.UI; // Thư viện cho UI nếu cần
using Keyboard; // Đảm bảo rằng namespace này đúng và có chứa class KeyboardManager
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using multiplayerMode;
namespace multiplayerMode
{
public class Login : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    [SerializeField]
    private GameObject virtualKeyBoard;
    private KeyboardManager keyboardManager; // Đảm bảo KeyboardManager được khai báo và hoạt động trong dự án của bạn

    private string sharedGroupId = "PublicUsernames"; // ID của nhóm chia sẻ

    private void Start()
    {
        virtualKeyBoard.SetActive(false);

        // Đăng ký sự kiện khi trường nhập được chọn
        usernameInput.onSelect.AddListener(delegate { HandleInputSelected(usernameInput); });
        passwordInput.onSelect.AddListener(delegate { HandleInputSelected(passwordInput); });
    }

    private void HandleInputSelected(TMP_InputField selectedInputField)
    {
        virtualKeyBoard.SetActive(true);
        keyboardManager = virtualKeyBoard.GetComponent<KeyboardManager>();
        if (keyboardManager != null)
        {
            keyboardManager.PlayerChooseInput(selectedInputField);
        }
        else
        {
            Debug.LogError("KeyboardManager not assigned or not found in the project");
        }
    }

    // Hàm xử lý nút đăng nhập
    public void LoginBtn()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = usernameInput.text,
            Password = passwordInput.text
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Đăng nhập thành công!");
        // Chuyển đổi đến scene có index 1
        SceneManager.LoadScene(1);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Đăng nhập thất bại: " + error.GenerateErrorReport());
    }

    // Hàm xử lý nút đăng ký
    public void RegisterBtn()
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = usernameInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Đăng ký thành công!");

        // Lấy tên người dùng từ email (loại bỏ phần sau @)
        string username = usernameInput.text.Split('@')[0];

        // Gọi hàm thêm tên người dùng vào danh sách public
        AddUsernameToSharedGroup(username);

        // Thực hiện các thao tác sau khi đăng ký thành công, ví dụ như đăng nhập tự động
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError("Đăng ký thất bại: " + error.GenerateErrorReport());
    }

    private void AddUsernameToSharedGroup(string username)
    {
        // Tạo nhóm chia sẻ hoặc bỏ qua nếu nhóm đã tồn tại
        var createGroupRequest = new CreateSharedGroupRequest
        {
            SharedGroupId = sharedGroupId
        };

        PlayFabClientAPI.CreateSharedGroup(createGroupRequest,
            result =>
            {
                Debug.Log("Nhóm chia sẻ đã được tạo.");
                UpdateSharedGroupData(username);
            },
            error =>
            {
                // Bỏ qua lỗi nếu nhóm đã tồn tại, tiếp tục cập nhật dữ liệu
                Debug.Log("Nhóm chia sẻ đã tồn tại hoặc lỗi khác xảy ra, tiếp tục cập nhật dữ liệu...");
                UpdateSharedGroupData(username);
            });
    }

    private void UpdateSharedGroupData(string username)
    {
        // Lấy dữ liệu hiện có trong nhóm chia sẻ
        var getGroupDataRequest = new GetSharedGroupDataRequest
        {
            SharedGroupId = sharedGroupId
        };

        PlayFabClientAPI.GetSharedGroupData(getGroupDataRequest,
            result =>
            {
                string existingUsernames;
                if (result.Data.ContainsKey("Usernames"))
                {
                    existingUsernames = result.Data["Usernames"].Value + "," + username;
                }
                else
                {
                    existingUsernames = username;
                }

                // Cập nhật dữ liệu trong nhóm chia sẻ
                var updateGroupDataRequest = new UpdateSharedGroupDataRequest
                {
                    SharedGroupId = sharedGroupId,
                    Data = new Dictionary<string, string>
                    {
                        { "Usernames", existingUsernames }
                    }
                };

                PlayFabClientAPI.UpdateSharedGroupData(updateGroupDataRequest,
                    success => Debug.Log("Cập nhật nhóm chia sẻ thành công!"),
                    error => Debug.LogError("Cập nhật nhóm chia sẻ thất bại: " + error.GenerateErrorReport()));
            },
            error => Debug.LogError("Lấy dữ liệu nhóm chia sẻ thất bại: " + error.GenerateErrorReport()));
    }
}
}

