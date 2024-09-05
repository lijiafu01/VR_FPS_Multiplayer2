using System.Collections.Generic;
using multiplayerMode;
namespace multiplayerMode
{
public static class FakeDatabase
{
    // Danh sách người dùng trong hệ thống
    public static Dictionary<string, User> Users = new Dictionary<string, User>()
    {
        { "user1", new User("user1") },
        { "user2", new User("user2") },
        { "user3", new User("user3") },
        { "aaaaaa1", new User("aaaaaa1") }  // Thêm người dùng giả lập vào cơ sở dữ liệu ảo
    };
}

public class User
{
    public string Username;
    public List<string> Friends;
    public List<string> FriendRequestsReceived;

    public User(string username)
    {
        Username = username;
        Friends = new List<string>();
        FriendRequestsReceived = new List<string>();
    }
}

}
