using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class UserData : NetworkBehaviour
{
    // Sử dụng Networked để đồng bộ nickname giữa các client
    [Networked] public NetworkString<_16> NickName { get; set; }

    // Hàm này sẽ được gọi khi đối tượng được spawn
    public override void Spawned()
    {
        // Nếu client này có quyền InputAuthority (nghĩa là người chơi local đang điều khiển đối tượng này)
        if (HasInputAuthority)
        {
            // Gọi RPC để gửi nickname của người chơi này tới server
            Rpc_SetNickname(GameManager.Instance.PlayerData.playerName);
        }

        // Liên kết đối tượng với người chơi có InputAuthority
        Runner.SetPlayerObject(Object.InputAuthority, Object);

        // Nếu client này có quyền StateAuthority (thường là server hoặc host)
        if (HasStateAuthority)
        {
            // Đồng bộ nickname nếu có quyền StateAuthority
            NickName = GameManager.Instance.PlayerData.playerName;
        }

        // Liên kết đối tượng với người chơi có StateAuthority
        Runner.SetPlayerObject(Object.StateAuthority, Object);
    }

    // RPC này sẽ được gọi từ client có InputAuthority và gửi nickname đến server
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_SetNickname(string nickname)
    {
        // Cập nhật nickname trên server và đồng bộ với các client khác
        NickName = nickname;
    }
}
