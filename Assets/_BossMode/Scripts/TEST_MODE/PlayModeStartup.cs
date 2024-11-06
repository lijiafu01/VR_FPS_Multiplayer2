#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PlayModeStartup
{
    static PlayModeStartup()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            Debug.Log("Nút Play được nhấn, chạy hàm đặc biệt cho Play Mode trong Editor.");
            RunYourFunction();
        }
    }
    private static void RunYourFunction()
    {
        if (SceneManager.GetActiveScene().name != "Login")
        {
            SceneManager.LoadScene("Login");
        }
    }
}
#endif
