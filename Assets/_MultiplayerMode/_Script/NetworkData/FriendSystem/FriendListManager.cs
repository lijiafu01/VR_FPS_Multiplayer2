using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FriendListManager : MonoBehaviour
{
    public GameObject friendTemplate;  // Template hiển thị tên bạn bè
    public Transform friendListContent;  // Content của ScrollView để hiển thị danh sách bạn bè

    private void Start()
    {
        LoadFriendList();
    }

    public void LoadFriendList()
    {
        foreach (Transform child in friendListContent)
        {
            Destroy(child.gameObject);
        }

        User currentUser = FakeDatabase.Users["CurrentUser"];  // Thay "CurrentUser" bằng người dùng hiện tại

        foreach (string friendUsername in currentUser.Friends)
        {
            GameObject newEntry = Instantiate(friendTemplate, friendListContent);
            newEntry.SetActive(true);

            TMP_Text nameText = newEntry.transform.Find("NameText").GetComponent<TMP_Text>();
            nameText.text = friendUsername;
        }
    }
}
