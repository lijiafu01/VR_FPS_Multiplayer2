using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

namespace multiplayerMode
{
    public class FriendSearchManager : MonoBehaviour
    {
        public TMP_InputField searchInputField;  // Input Field để nhập tên người chơi
        public Button searchButton;  // Nút để thực hiện tìm kiếm
        public GameObject friendTemplate;  // Template hiển thị thông tin người chơi và nút Add
        public Transform searchResultsContent;  // Content của ScrollView để hiển thị kết quả

        private void Start()
        {
            searchInputField.text = "aaaaaak";
            searchButton.onClick.AddListener(OnSearchButtonClicked);
        }

        // Khi nhấn nút Search
        private void OnSearchButtonClicked()
        {
            string searchQuery = searchInputField.text;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                // Tìm kiếm bạn bè qua PlayFab bằng TitleDisplayName
                SearchFriendByDisplayName(searchQuery);
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
                Debug.LogError("Friend not found or error occurred: " + error.GenerateErrorReport());
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
            addButton.onClick.AddListener(() => AddFriend(accountInfo.PlayFabId));
        }

        // Thêm người chơi vào danh sách bạn bè
        private void AddFriend(string playFabId)
        {
            PlayFabClientAPI.AddFriend(new AddFriendRequest
            {
                FriendPlayFabId = playFabId  // Dựa trên PlayFabId của người chơi
            },
            result =>
            {
                Debug.Log("Friend added successfully!");
            },
            error =>
            {
                Debug.LogError("Failed to add friend: " + error.GenerateErrorReport());
            });
        }
    }
}
