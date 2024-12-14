using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float deltaTime = 0.0f; // Tổng thời gian giữa các khung hình
    private float frameCount = 0;   // Tổng số khung hình trong khoảng thời gian
    private float timer = 0.0f;     // Bộ đếm thời gian

    [SerializeField]
    private float interval = 5.0f;  // Thời gian tính trung bình (mặc định 5 giây)

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject); // Đảm bảo không phá hủy đối tượng khi chuyển scene
    }

    void Update()
    {
        // Cộng dồn thời gian giữa các khung hình
        deltaTime += Time.unscaledDeltaTime;

        // Tăng số lượng khung hình
        frameCount++;

        // Tăng bộ đếm thời gian
        timer += Time.unscaledDeltaTime;

        // Nếu vượt quá khoảng thời gian cài đặt
        if (timer >= interval)
        {
            // Tính FPS trung bình
            float averageFPS = frameCount / deltaTime;

            // Reset lại giá trị cho chu kỳ tiếp theo
            deltaTime = 0.0f;
            frameCount = 0;
            timer = 0.0f;

            // Ghi log FPS trung bình
            Debug.Log("Average FPS over " + interval + " seconds: " + averageFPS.ToString("0.0"));
        }
    }
}
