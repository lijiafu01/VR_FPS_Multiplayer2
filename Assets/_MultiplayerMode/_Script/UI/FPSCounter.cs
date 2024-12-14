using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    //public Text fpsText;

    private float deltaTime = 0.0f;
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Update()
    {
        // Tính toán thời gian giữa các khung hình
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // Tính FPS
        float fps = 1.0f / deltaTime;
        Debug.Log("FPS counter: " + string.Format("FPS: {0:0.}", fps));
        // Hiển thị FPS lên UI Text
        //fpsText.text = string.Format("FPS: {0:0.}", fps);
    }
}
