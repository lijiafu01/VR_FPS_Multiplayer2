using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Thư viện cho TextMeshPro
using UnityEngine.UI; // Thư viện cho UI nếu cần
using Keyboard; // Đảm bảo rằng namespace này đúng và có chứa class KeyboardManager
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    [SerializeField]
    private GameObject virtualKeyBoard;
    private KeyboardManager keyboardManager; // Đảm bảo KeyboardManager được khai báo và hoạt động trong dự án của bạn

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
        // Thực hiện các thao tác sau khi đăng ký thành công, ví dụ như đăng nhập tự động
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError("Đăng ký thất bại: " + error.GenerateErrorReport());
    }
}
