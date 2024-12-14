using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
namespace multiplayerMode
{
    public class Login : MonoBehaviour
    {
        public NoticeText _noticeText;
        private string _displayName;
        [SerializeField] private Toggle toggle; // Tham chiếu đến Toggle trong Inspector
        public TMP_InputField usernameInput;    // InputField cho tài khoản
        public TMP_InputField passwordInput;    // InputField cho mật khẩu
        private bool toggleState = true;        // Giá trị khởi đầu là true
        private void Start()
        {
            // Kiểm tra nếu Toggle và InputField đã được gán
            if (toggle == null || usernameInput == null || passwordInput == null)
            {
                Debug.LogError("Missing references in the Inspector.");
                return;
            }
            // Lấy dữ liệu đã lưu nếu có
            if (PlayerPrefs.HasKey("ToggleState"))
            {
                toggleState = PlayerPrefs.GetInt("ToggleState") == 1; // Lấy trạng thái Toggle
                toggle.isOn = toggleState;
                if (toggleState)
                {
                    usernameInput.text = PlayerPrefs.GetString("SavedUsername", "");
                    passwordInput.text = PlayerPrefs.GetString("SavedPassword", "");
                }
            }
            // Đăng ký sự kiện thay đổi trạng thái Toggle
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
        private void OnToggleValueChanged(bool isOn)
        {
            toggleState = isOn; // Cập nhật trạng thái Toggle
        }
        void SaveAccount()
        {
            // Lưu trạng thái Toggle
            PlayerPrefs.SetInt("ToggleState", toggleState ? 1 : 0);
            if (toggleState)
            {
                // Lưu tài khoản và mật khẩu nếu Toggle đang bật
                PlayerPrefs.SetString("SavedUsername", usernameInput.text);
                PlayerPrefs.SetString("SavedPassword", passwordInput.text);
            }
            else
            {
                // Xóa tài khoản và mật khẩu nếu Toggle tắt
                PlayerPrefs.DeleteKey("SavedUsername");
                PlayerPrefs.DeleteKey("SavedPassword");
            }
            PlayerPrefs.Save(); // Lưu lại toàn bộ dữ liệu
        }
        private void OnDestroy()
        {
            // Hủy đăng ký sự kiện khi đối tượng bị phá hủy
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
            }
        }
        public void SwitchAccount()
        {
            usernameInput.text = "mike@gmail.com";
            passwordInput.text = "aaaaaa";
        }
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
            SaveAccount();
            // Lấy _displayName từ kết quả đăng nhập
            _displayName = result.InfoResultPayload.PlayerProfile.DisplayName;
            PlayFabManager.Instance.UserData.Email = usernameInput.text;
            PlayFabManager.Instance.UserData.Password = passwordInput.text;
            PlayFabManager.Instance.UserData.DisplayName = _displayName;
            PlayFabManager.Instance.UserData.UserID = result.PlayFabId;
            PlayFabManager.Instance.UserData.LoadAttributesFromPlayFab();
        }
        private void OnLoginFailure(PlayFabError error)
        {
            _noticeText.DisplayText("Login failed!");
            Debug.LogError("Đăng nhập thất bại: " + error.GenerateErrorReport());
        }
        // Hàm xử lý nút đăng ký
        public void RegisterBtn()
        {
            string username = usernameInput.text;

            // Tìm vị trí của '@' trong username
            int atIndex = username.IndexOf('@');

            // Nếu có '@', lấy độ dài chuỗi trước '@'. Nếu không có '@', dùng toàn bộ tên
            int lengthBeforeAt = (atIndex >= 0) ? atIndex : username.Length;

            if (lengthBeforeAt < 6)
            {
                _noticeText.DisplayText("Registration failed!");
                return;
            }

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
            _noticeText.DisplayText("Registration successful!");
            // Lấy tên người dùng từ email (loại bỏ phần sau @)
            string username = usernameInput.text.Split('@')[0];
            // Cập nhật TitleDisplayName với tên đã lấy từ email
            UpdateTitleDisplayName(username);
            // Khởi tạo dữ liệu thuộc tính mặc định
            PlayFabManager.Instance.UserData.InitializeDefaultAttributes();
            // Gọi hàm SaveAttributesToPlayFab() từ UserData
            PlayFabManager.Instance.UserData.SaveAttributesToPlayFab();
        }
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
        private void OnDisplayNameUpdateSuccess(UpdateUserTitleDisplayNameResult result)
        {
        }
        private void OnDisplayNameUpdateFailure(PlayFabError error)
        {
            Debug.LogError("Failed to update TitleDisplayName: " + error.GenerateErrorReport());
        }
    }
}
