using TMPro;
using UnityEngine;
public class PlayerListCell : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _playerNameTxt;
    public void SetPlayerName(string playerName)
    {
        _playerNameTxt.text = playerName;
    }
}