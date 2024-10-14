using Fusion;
using multiplayerMode;
using UnityEngine;

public class PlayerNetworkData : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnPlayerNameChanged))]
    public string PlayerName { get; set; }
    


    public override void Spawned()
    {
        Debug.Log("PlayerNetworkData_da duoc sinh ra");
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
  
   
    private static void OnPlayerNameChanged(Changed<PlayerNetworkData> changed)
    {
        NetworkManager.Instance.UpdatePlayerList();
    }
}