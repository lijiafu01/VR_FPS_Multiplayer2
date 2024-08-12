using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Thư viện cho TextMeshPro
using UnityEngine.UI; // Thư viện cho UI nếu cần
using Keyboard; // Đảm bảo rằng namespace này đúng và có chứa class KeyboardManager
using PlayFab;


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
    public void LoginBtn()
    {

    }
    public void RegisterBtn()
    {

    }
}
