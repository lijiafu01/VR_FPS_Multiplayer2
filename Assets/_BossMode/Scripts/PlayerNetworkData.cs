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
            NetworkManager.Instance._playerRef = Object.InputAuthority;
        }
       
        NetworkManager.Instance.SetPlayerNetworkData(Object.InputAuthority, this);
    }
    /*public void MidFuntion_BossStart()
    {
        if(Object.HasInputAuthority)
        {
            StartGame();
        }
    }*/


    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void SetPlayerName_RPC(string playerName)
    {
        PlayerName = playerName;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void StartGame_RPC(string teamID)
    {
        
        NetworkManager.Instance.UpdateTeamName(teamID);
        NetworkManager.Instance.MidFuntion_StartBossScene();
        
    }
    /*[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void SetTeamName_RPC(string teamName)
    {
        NetworkManager.Instance.UpdateTeamName(teamName);
    }*/
   
    private static void OnPlayerNameChanged(Changed<PlayerNetworkData> changed)
    {
        NetworkManager.Instance.UpdatePlayerList();
    }
}