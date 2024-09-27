using multiplayerMode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLogin : MonoBehaviour
{
    public Login login;
   
    private void Start()
    {
        if(PlayFabManager.Instance.isPC)
        {
            LoginPC();
        }
        else
        {
            LoginQuest();
        }
    }
    public void LoginPC()
    {
        PlayFabManager.Instance.isPC = true;
        login.usernameInput.text = "aaaaaad@gmail.com";
        login.passwordInput.text = "aaaaaa";
        login.LoginBtn();
    }
    public void LoginQuest()
    {
        PlayFabManager.Instance.isPC = false;

        login.usernameInput.text = "aaaaaaf@gmail.com";
        login.passwordInput.text = "aaaaaa";
        login.LoginBtn();
    }
}
