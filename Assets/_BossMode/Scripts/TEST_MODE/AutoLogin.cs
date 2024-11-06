using multiplayerMode;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AutoLogin : MonoBehaviour
{
    
    public Login login;
    private void Start()
    {
       
        if (Application.platform == RuntimePlatform.WindowsEditor)
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
        login.usernameInput.text = "tony2@gmail.com";
        login.passwordInput.text = "aaaaaa";
        login.LoginBtn();
    }
    public void LoginQuest()
    {
        login.usernameInput.text = "jack2@gmail.com";
        login.passwordInput.text = "aaaaaa";
        login.LoginBtn();
    }


}
