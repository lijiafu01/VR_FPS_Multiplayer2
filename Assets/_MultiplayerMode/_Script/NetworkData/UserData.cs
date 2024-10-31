using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace multiplayerMode
{
    public class UserData : MonoBehaviour
    {
        // Thay vì chỉ lưu trữ một `AttributeData`, chúng ta sử dụng Dictionary
        public Dictionary<string, AttributeData> PlayerAttributes;

        public string UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }

        public void SetAccount(string email, string passW)
        {
            Email = email;
            Password = passW;
        }
        public void UpgradePlayerAttributeData(string heroName, AttributeData updatedData)
        {
            if (PlayerAttributes == null)
            {
                Debug.LogError("PlayerAttributes chưa được khởi tạo.");
                return;
            }

            if (PlayerAttributes.ContainsKey(heroName))
            {
                // Cập nhật dữ liệu của nhân vật
                PlayerAttributes[heroName] = updatedData;

                // Lưu dữ liệu cập nhật lên PlayFab
                SaveAttributesToPlayFab();
            }
            else
            {
                Debug.LogError($"Không tìm thấy nhân vật với tên {heroName} trong PlayerAttributes.");
            }
        }

        // Hàm tải dữ liệu thuộc tính từ PlayFab
        public void LoadAttributesFromPlayFab()
        {
            if (string.IsNullOrEmpty(UserID))
            {
                Debug.LogError("Chưa đăng nhập. Không thể tải dữ liệu từ PlayFab.");
                return;
            }

            PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataLoadSuccess, OnDataLoadError);
        }

        private void OnDataLoadSuccess(GetUserDataResult result)
        {
            if (result.Data != null && result.Data.ContainsKey("AttributeData"))
            {
                string jsonAttributes = result.Data["AttributeData"].Value;
                Debug.Log("JSON nhận được từ PlayFab: " + jsonAttributes);

                // Deserialize JSON thành Dictionary<string, AttributeData>
                PlayerAttributes = JsonUtility.FromJson<SerializableDictionary<string, AttributeData>>(jsonAttributes).ToDictionary();
                Debug.Log("LoadAttributesFromPlayFab: Dữ liệu đã được gán vào PlayerAttributes.");

                // Chuyển cảnh sau khi dữ liệu đã tải xong
                SceneManager.LoadScene(1);
            }
            else
            {
                Debug.Log("Không tìm thấy dữ liệu thuộc tính người chơi trên PlayFab.");
                // Khởi tạo dữ liệu mặc định
                //InitializeDefaultAttributes();

                // Chuyển cảnh
                SceneManager.LoadScene(1);
            }
        }

        private void OnDataLoadError(PlayFabError error)
        {
            Debug.LogError("Lỗi khi tải dữ liệu từ PlayFab: " + error.GenerateErrorReport());
            // Khởi tạo dữ liệu mặc định
            //InitializeDefaultAttributes();

            // Chuyển cảnh
            SceneManager.LoadScene(1);
        }

        // Hàm lưu dữ liệu thuộc tính lên PlayFab
        public void SaveAttributesToPlayFab()
        {
            if (PlayerAttributes == null)
            {
                Debug.LogError("PlayerAttributes chưa được khởi tạo.");
                return;
            }

            // Chuyển đổi Dictionary thành JSON
            string jsonAttributes = JsonUtility.ToJson(new SerializableDictionary<string, AttributeData>(PlayerAttributes));

            var request = new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    { "AttributeData", jsonAttributes }
                }
            };

            PlayFabClientAPI.UpdateUserData(request, OnDataSendSuccess, OnDataSendError);
        }

        private void OnDataSendSuccess(UpdateUserDataResult result)
        {
            Debug.Log("AttributeData: Dữ liệu thuộc tính đã được lưu lên PlayFab thành công!");
        }

        private void OnDataSendError(PlayFabError error)
        {
            Debug.LogError("AttributeData: Lỗi khi lưu dữ liệu lên PlayFab: " + error.GenerateErrorReport());
        }

        // Hàm khởi tạo dữ liệu mặc định
        public void InitializeDefaultAttributes()
        {
            PlayerAttributes = new Dictionary<string, AttributeData>();

            // Thêm các nhân vật mặc định với thuộc tính ban đầu
            PlayerAttributes.Add("Police", new AttributeData(0, 0, 0,1000,50,50));
            PlayerAttributes.Add("Angel", new AttributeData(0, 0, 0, 1000, 100, 45));
            //PlayerAttributes.Add("Hero2", new AttributeData(1, 1, 1, 120, 40, 12));
            // Thêm các nhân vật khác nếu cần
        }
    }
}
