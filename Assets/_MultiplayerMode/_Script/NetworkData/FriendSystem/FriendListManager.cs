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

        private void Start()
        {
            GetFriends();  // Lấy danh sách bạn bè khi bắt đầu
        }

        // Lấy danh sách bạn bè từ PlayFab
        private void GetFriends()
        {
            PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest(),
            result =>
            {
                _friends = result.Friends;  // Lưu danh sách bạn bè
                DisplayFriends();  // Hiển thị danh sách bạn bè lên giao diện
            },
            error =>
            {
                Debug.LogError("Failed to get friends list: " + error.GenerateErrorReport());
            });
        }

        // Hiển thị danh sách bạn bè trong giao diện
        private void DisplayFriends()
        {
            foreach (var friend in _friends)
            {
                // Tạo đối tượng giao diện dựa trên friendTemplate
                GameObject newFriendEntry = Instantiate(friendTemplate, friendListContent);
                newFriendEntry.SetActive(true);

                // Cập nhật tên của bạn bè trong template
                TMP_Text nameText = newFriendEntry.transform.Find("NameText").GetComponent<TMP_Text>();

                // Hiển thị PlayFabId hoặc TitleDisplayName (tên trước dấu '@' của email nếu có)
                string friendName = !string.IsNullOrEmpty(friend.TitleDisplayName)
                    ? friend.TitleDisplayName
                    : friend.FriendPlayFabId;  // Nếu không có TitleDisplayName, dùng PlayFabId
                nameText.text = friendName;  // Hiển thị tên bạn bè hoặc PlayFabId nếu không có tên hiển thị

                // Xử lý nút "Xóa bạn"
                Button removeButton = newFriendEntry.transform.Find("RemoveButton").GetComponent<Button>();
                removeButton.onClick.AddListener(() => RemoveFriend(friend.FriendPlayFabId, newFriendEntry));
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
