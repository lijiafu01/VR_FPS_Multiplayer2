using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine.UI;
namespace multiplayerMode
{
    public class FriendListManager : MonoBehaviour
    {
        public GameObject friendTemplate;  // Template hiển thị tên bạn bè
        public Transform friendListContent;  // Content của ScrollView để hiển thị danh sách bạn bè
        private List<FriendInfo> _friends = new List<FriendInfo>();  // Danh sách bạn bè
        private void OnEnable()
        {
            GetFriends();  // Lấy danh sách bạn bè khi bắt đầu
        }
        // Lấy danh sách bạn bè từ PlayFab
        private void GetFriends()
        {
            PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(),
            result =>
            {
                foreach (var friend in result.Friends)
                {
                    string friendPlayFabId = friend.FriendPlayFabId;
                    string friendUsername = friend.Username; // Nếu có
                    string friendDisplayName = friend.TitleDisplayName; // Nếu có

                    Debug.Log("Friend PlayFab ID: " + friendPlayFabId);
                    // Bạn có thể lưu trữ hoặc hiển thị danh sách bạn bè để người chơi chọn
                }
                _friends = result.Friends;  // Lưu danh sách bạn bè
                DisplayFriends();  // Hiển thị danh sách bạn bè lên giao diện
            },
            error =>
            {
                Debug.LogError("Failed to get friends list: " + error.GenerateErrorReport());
            });
        }
        private void DisplayFriends()
        {
            // Xóa các mục bạn bè hiện tại để tránh trùng lặp
            foreach (Transform child in friendListContent)
            {
                Destroy(child.gameObject);
            }
            foreach (var friend in _friends)
            {
                // Tạo đối tượng giao diện dựa trên friendTemplate
                GameObject newFriendEntry = Instantiate(friendTemplate, friendListContent);
                newFriendEntry.SetActive(true);
                // Cập nhật tên của bạn bè trong template
                Transform nameTextTransform = newFriendEntry.transform.Find("NameText");
                if (nameTextTransform != null)
                {
                    TMP_Text nameText = nameTextTransform.GetComponent<TMP_Text>();
                    if (nameText != null)
                    {
                        // Hiển thị TitleDisplayName hoặc Username hoặc PlayFabId nếu không có TitleDisplayName
                        string friendName = !string.IsNullOrEmpty(friend.TitleDisplayName)
                            ? friend.TitleDisplayName
                            : (!string.IsNullOrEmpty(friend.Username) ? friend.Username : friend.FriendPlayFabId);
                        nameText.text = friendName;
                    }
                    else
                    {
                        Debug.LogError("dev3_NameText component not found in friendTemplate.");
                    }
                }
                else
                {
                    Debug.LogError("dev3_NameText transform not found in friendTemplate.");
                }
                // Xử lý nút "Xóa bạn"
                Transform removeButtonTransform = newFriendEntry.transform.Find("RemoveButton");
                if (removeButtonTransform != null)
                {
                    Button removeButton = removeButtonTransform.GetComponent<Button>();
                    if (removeButton != null)
                    {
                        removeButton.onClick.AddListener(() => RemoveFriend(friend.FriendPlayFabId, newFriendEntry));
                    }
                    else
                    {
                        Debug.LogError("dev3_RemoveButton component not found in friendTemplate.");
                    }
                }
                else
                {
                    Debug.LogError("dev3_RemoveButton transform not found in friendTemplate.");
                }
            }
        }
        // Hàm xóa bạn bè dựa trên PlayFabId
        private void RemoveFriend(string playFabId, GameObject friendEntry)
        {
            PlayFabClientAPI.RemoveFriend(new RemoveFriendRequest
            {
                FriendPlayFabId = playFabId
            },
            result =>
            {
                Debug.Log("Friend removed successfully!");

                // Gỡ đối tượng bạn bè ra khỏi giao diện
                Destroy(friendEntry);

                // Xóa bạn khỏi danh sách cục bộ
                _friends.RemoveAll(f => f.FriendPlayFabId == playFabId);
            },
            error =>
            {
                Debug.LogError("Failed to remove friend: " + error.GenerateErrorReport());
            });
        }
    }
}
