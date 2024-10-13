using multiplayerMode;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerTeamSetup : MonoBehaviour
{
    public TMP_Text playerNameText;
    public void SetPlayerNameColor(string TeamID)
    {
        if (NetworkManager.Instance.TeamID != null)
        {
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
