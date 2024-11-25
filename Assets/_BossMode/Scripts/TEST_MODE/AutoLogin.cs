using multiplayerMode;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AutoLogin : MonoBehaviour
{
    public Login login;
    bool isPC = Application.platform == RuntimePlatform.WindowsEditor;
    public void LoginPC()
    {
        login.usernameInput.text = "mike@gmail.com";
        login.passwordInput.text = "aaaaaa";
        login.LoginBtn();
    }
    public void LoginQuest()
    {
        login.usernameInput.text = "trump1@gmail.com";
        login.passwordInput.text = "aaaaaa";
        login.LoginBtn();
    }
}
