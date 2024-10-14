using multiplayerMode;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerTeamSetup : MonoBehaviour
{
    public TMP_Text playerNameText;
    public string teamID { get; set; }
    public void SetPlayerNameColor(string TeamID)
    {
        if (NetworkManager.Instance.TeamID != null)
        {
            teamID = TeamID;
            if (TeamID != NetworkManager.Instance.TeamID)
            {
                playerNameText.color = Color.red;
            }
            else
            {
                playerNameText.color = Color.green;
            }
        }
    }
}
