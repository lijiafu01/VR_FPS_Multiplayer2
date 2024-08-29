using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FriendRequestManager : MonoBehaviour
{
    public GameObject requestTemplate;  // Template hiển thị yêu cầu kết bạn
    public Transform requestListContent;  // Content của ScrollView để hiển thị danh sách yêu cầu kết bạn

    private void Start()
    {
        LoadFriendRequests();
    }

    public void LoadFriendRequests()
    {
        foreach (Transform child in requestListContent)
        {
            Destroy(child.gameObject);
        }

        User currentUser = FakeDatabase.Users["CurrentUser"];  // Thay "CurrentUser" bằng người dùng hiện tại

        foreach (string requesterUsername in currentUser.FriendRequestsReceived)
        {
            GameObject newEntry = Instantiate(requestTemplate, requestListContent);
            newEntry.SetActive(true);

            TMP_Text nameText = newEntry.transform.Find("NameText").GetComponent<TMP_Text>();
            nameText.text = requesterUsername;

            Button acceptButton = newEntry.transform.Find("AcceptButton").GetComponent<Button>();
            Button rejectButton = newEntry.transform.Find("RejectButton").GetComponent<Button>();

            acceptButton.onClick.AddListener(() => AcceptFriendRequest(requesterUsername));
            rejectButton.onClick.AddListener(() => RejectFriendRequest(requesterUsername));
        }
    }

    private void AcceptFriendRequest(string requesterUsername)
    {
        User currentUser = FakeDatabase.Users["CurrentUser"];
        if (!currentUser.Friends.Contains(requesterUsername))
        {
            currentUser.Friends.Add(requesterUsername);
            FakeDatabase.Users[requesterUsername].Friends.Add("CurrentUser");
            currentUser.FriendRequestsReceived.Remove(requesterUsername);
            Debug.Log("Đã chấp nhận lời mời kết bạn từ " + requesterUsername);
        }

        LoadFriendRequests();
    }

    private void RejectFriendRequest(string requesterUsername)
    {
        User currentUser = FakeDatabase.Users["CurrentUser"];
        if (currentUser.FriendRequestsReceived.Contains(requesterUsername))
        {
            currentUser.FriendRequestsReceived.Remove(requesterUsername);
            Debug.Log("Đã từ chối lời mời kết bạn từ " + requesterUsername);
        }

        LoadFriendRequests();
    }
}
