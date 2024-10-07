using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

namespace multiplayerMode
{
    public class FriendSearchManager : MonoBehaviour
    {
        public GameObject addTab;
        public TMP_InputField searchInputField;  // Input Field để nhập tên người chơi
        public Button searchButton;  // Nút để thực hiện tìm kiếm
        public GameObject friendTemplate;  // Template hiển thị thông tin người chơi và nút Add
        public Transform searchResultsContent;  // Content của ScrollView để hiển thị kết quả

        private void OnEnable()
        {
            addTab.SetActive(false);
            searchInputField.text = ""; // Đặt giá trị mặc định trống
            searchButton.onClick.AddListener(OnSearchButtonClicked);
        }
        private void OnDisable()
        {
            // Loại bỏ listener khi giao diện bị tắt để tránh trùng lặp
            searchButton.onClick.RemoveListener(OnSearchButtonClicked);
        }
        // Khi nhấn nút Search
        private void OnSearchButtonClicked()
        {
            string searchQuery = searchInputField.text.Trim();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                // Xóa kết quả tìm kiếm trước đó để tránh trùng lặp
                ClearSearchResults();

                // Tìm kiếm bạn bè qua PlayFab bằng TitleDisplayName
                SearchFriendByDisplayName(searchQuery);
            }
            else
            {
                Debug.LogWarning("dev3_Search query is empty.");
            }
        }

        // Hàm xóa các kết quả tìm kiếm hiện tại
        private void ClearSearchResults()
        {
            foreach (Transform child in searchResultsContent)
            {
                Destroy(child.gameObject);
            }
        }

        // Tìm kiếm bạn bè dựa trên TitleDisplayName
        private void SearchFriendByDisplayName(string displayName)
        {
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest
            {
                TitleDisplayName = displayName  // Tìm kiếm bằng TitleDisplayName
            },
            result =>
            {
                // Nếu tìm thấy người chơi, hiển thị kết quả
                DisplaySearchResult(result.AccountInfo);
            },
            error =>
            {
                Debug.LogError("dev3_Friend not found or error occurred: " + error.GenerateErrorReport());
                // Hiển thị thông báo người dùng không tìm thấy bạn bè
                DisplayNoFriendFound();
            });
        }

        // Hiển thị kết quả tìm kiếm
        private void DisplaySearchResult(UserAccountInfo accountInfo)
        {
            // Tạo đối tượng giao diện dựa trên friendTemplate
            GameObject newFriendEntry = Instantiate(friendTemplate, searchResultsContent);
            newFriendEntry.SetActive(true);

            // Cập nhật thông tin người chơi trong template (hiển thị TitleDisplayName)
            TMP_Text nameText = newFriendEntry.transform.Find("NameText").GetComponent<TMP_Text>();
            nameText.text = accountInfo.TitleInfo.DisplayName;  // Cập nhật TitleDisplayName

            // Xử lý nút "Add Friend"
            Button addButton = newFriendEntry.transform.Find("AddButton").GetComponent<Button>();
            addButton.onClick.AddListener(() => AddFriend(accountInfo.PlayFabId, newFriendEntry));
        }

        // Hiển thị thông báo không tìm thấy bạn bè
        private void DisplayNoFriendFound()
        {
            // Tạo một entry thông báo không tìm thấy bạn bè
            GameObject noFriendEntry = Instantiate(friendTemplate, searchResultsContent);
            noFriendEntry.SetActive(true);

            // Cập nhật thông báo
            TMP_Text nameText = noFriendEntry.transform.Find("NameText").GetComponent<TMP_Text>();
            nameText.text = "Friend not found.";

            // Ẩn nút Add Friend
            Button addButton = noFriendEntry.transform.Find("AddButton").GetComponent<Button>();
            if (addButton != null)
            {
                addButton.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("dev3_AddButton component not found in friendTemplate.");
            }
        }

        // Thêm người chơi vào danh sách bạn bè
        private void AddFriend(string playFabId, GameObject friendEntry)
        {
            PlayFabClientAPI.AddFriend(new AddFriendRequest
            {
                FriendPlayFabId = playFabId  // Dựa trên PlayFabId của người chơi
            },
            result =>
            {
                Debug.Log("dev4_Friend added successfully!");

                addTab.SetActive(true);
                Destroy(friendEntry);
            },
            error =>
            {
                Debug.LogError("Failed to add friend: " + error.GenerateErrorReport());

                // Hiển thị thông báo lỗi nếu cần (có thể thêm UI popup nếu cần)
            });
        }
    }
}
