using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
using Keyboard;

namespace multiplayerMode
{
    public class Login : MonoBehaviour
    {
        public NoticeText _noticeText;
        private string _displayName;
        public TMP_InputField usernameInput;
        public TMP_InputField passwordInput;
        [SerializeField]
        //private GameObject virtualKeyBoard;
        private KeyboardManager keyboardManager; // Đảm bảo KeyboardManager được khai báo và hoạt động trong dự án của bạn

       
        /* private void Start()
         {
             //virtualKeyBoard.SetActive(false);

             // Đăng ký sự kiện khi trường nhập được chọn
             usernameInput.onSelect.AddListener(delegate { HandleInputSelected(usernameInput); });
             passwordInput.onSelect.AddListener(delegate { HandleInputSelected(passwordInput); });
         }*/
        public void SwitchAccount()
        {
            usernameInput.text = "mike@gmail.com";
        }
        /*private void HandleInputSelected(TMP_InputField selectedInputField)
        {
            //virtualKeyBoard.SetActive(true);
            keyboardManager = virtualKeyBoard.GetComponent<KeyboardManager>();
            if (keyboardManager != null)
            {
                keyboardManager.PlayerChooseInput(selectedInputField);
            }
            else
            {
                Debug.LogError("KeyboardManager not assigned or not found in the project");
            }
        }*/

        // Hàm xử lý nút đăng nhập
        public void LoginBtn()
        {
            var request = new LoginWithEmailAddressRequest
            {
                Email = usernameInput.text,
                Password = passwordInput.text,
                InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
                {
                    GetPlayerProfile = true
                }
            };

            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }

        private void OnLoginSuccess(LoginResult result)
        {
            // Lấy _displayName từ kết quả đăng nhập
            _displayName = result.InfoResultPayload.PlayerProfile.DisplayName;
            PlayFabManager.Instance.UserData.Email = usernameInput.text;
            PlayFabManager.Instance.UserData.Password = passwordInput.text;
            PlayFabManager.Instance.UserData.DisplayName = _displayName;
            PlayFabManager.Instance.UserData.UserID = result.PlayFabId;

            PlayFabManager.Instance.UserData.LoadAttributesFromPlayFab();

            /*// Khởi tạo dữ liệu thuộc tính mặc định
            PlayFabManager.Instance.UserData.InitializeDefaultAttributes();
            // Gọi hàm SaveAttributesToPlayFab() từ UserData
            PlayFabManager.Instance.UserData.SaveAttributesToPlayFab();*/


            // Chuyển đổi đến scene có index 1          
            // SceneManager.LoadScene(1);
        }
        

        private void OnLoginFailure(PlayFabError error)
        {
            _noticeText.DisplayText("Login failed!");
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
            _noticeText.DisplayText("Registration successful!");

            /*_noticeText.gameObject.SetActive(false);
            _noticeText.gameObject.SetActive(true);
            _noticeText.text = "Registration successful!";*/

            // Lấy tên người dùng từ email (loại bỏ phần sau @)
            string username = usernameInput.text.Split('@')[0];

            // Cập nhật TitleDisplayName với tên đã lấy từ email
            UpdateTitleDisplayName(username);

            // Khởi tạo dữ liệu thuộc tính mặc định
            PlayFabManager.Instance.UserData.InitializeDefaultAttributes();

            // Gọi hàm SaveAttributesToPlayFab() từ UserData
            PlayFabManager.Instance.UserData.SaveAttributesToPlayFab();

            // Thực hiện các thao tác sau khi đăng ký thành công, ví dụ như đăng nhập tự động
        }

        /* private void OnDataSendSuccess(UpdateUserDataResult result)
         {
             Debug.Log("AttributeData_ Dữ liệu thuộc tính đã được lưu lên PlayFab thành công!");
         }

         private void OnDataSendError(PlayFabError error)
         {
             Debug.LogError("AttributeData_ Lỗi khi lưu dữ liệu lên PlayFab: " + error.GenerateErrorReport());
         }*/
        private void UpdateTitleDisplayName(string displayName)
        {
            var request = new UpdateUserTitleDisplayNameRequest
            {
                DisplayName = displayName
            };

            PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdateSuccess, OnDisplayNameUpdateFailure);
        }

        private void OnRegisterFailure(PlayFabError error)
        {
            _noticeText.DisplayText("Registration failed!");
            Debug.LogError("Đăng ký thất bại: " + error.GenerateErrorReport());
        }

        // Hàm cập nhật TitleDisplayName
       

        private void OnDisplayNameUpdateSuccess(UpdateUserTitleDisplayNameResult result)
        {

            Debug.Log("TitleDisplayName updated successfully to: " + result.DisplayName);
        }

        private void OnDisplayNameUpdateFailure(PlayFabError error)
        {
            Debug.LogError("Failed to update TitleDisplayName: " + error.GenerateErrorReport());
        }
    }
}
