using Fusion;
using multiplayerMode;

public class PlayerNetworkData : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnPlayerNameChanged))]
    public string PlayerName { get; set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            SetPlayerName_RPC(NetworkManager.Instance.PlayerName);
        }

        NetworkManager.Instance.SetPlayerNetworkData(Object.InputAuthority, this);
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void SetPlayerName_RPC(string playerName)
    {
        PlayerName = playerName;
    }

    private static void OnPlayerNameChanged(Changed<PlayerNetworkData> changed)
    {
        NetworkManager.Instance.UpdatePlayerList();
    }
}