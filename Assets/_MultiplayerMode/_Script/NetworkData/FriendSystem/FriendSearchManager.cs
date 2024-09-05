using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using multiplayerMode;
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
        searchInputField.text = "aaaaaa1";  // Xóa nội dung của Input Field
        searchButton.onClick.AddListener(SearchFriend);
        friendTemplate.SetActive(false);  // Đảm bảo rằng template gốc bị tắt để không hiển thị
    }

    private void SearchFriend()
    {
        string username = searchInputField.text.Trim();
        if (string.IsNullOrEmpty(username))
        {
            Debug.Log("Vui lòng nhập tên người chơi.");
            return;
        }

        if (FakeDatabase.Users.ContainsKey(username))
        {
            User foundUser = FakeDatabase.Users[username];
            DisplaySearchResult(foundUser);
        }
        else
        {
            Debug.Log("Không tìm thấy người dùng.");
        }
    }

    private void DisplaySearchResult(User user)
    {
        foreach (Transform child in searchResultsContent)
        {
            Destroy(child.gameObject);
        }

        GameObject newEntry = Instantiate(friendTemplate, searchResultsContent);
        newEntry.SetActive(true);

        TMP_Text nameText = newEntry.transform.Find("NameText").GetComponent<TMP_Text>();
        nameText.text = user.Username;

        Button addButton = newEntry.transform.Find("AddFriendButton").GetComponent<Button>();
        addButton.onClick.AddListener(() => SendFriendRequest(user.Username));
    }

    private void SendFriendRequest(string friendUsername)
    {
        if (!FakeDatabase.Users[friendUsername].FriendRequestsReceived.Contains("CurrentUser"))  // Thay "CurrentUser" bằng người dùng hiện tại
        {
            FakeDatabase.Users[friendUsername].FriendRequestsReceived.Add("CurrentUser");
            Debug.Log("Đã gửi lời mời kết bạn tới " + friendUsername);
        }
        else
        {
            Debug.Log("Đã gửi lời mời kết bạn trước đó.");
        }
    }
}
}

