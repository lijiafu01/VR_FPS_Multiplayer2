using UnityEngine;

public class OVRInputHandler : MonoBehaviour
{

    private void Update()
    {
        OVRInputState.Instance.TriggerPressed = GetTriggerPressed(); // Lấy giá trị của nút kích hoạt

      
    }
    public bool GetTriggerPressed()
    {
        // Lấy giá trị nút trigger bên trái
        float leftTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);

        // Lấy giá trị nút trigger bên phải
        float rightTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

        // Kiểm tra xem có nút trigger nào được nhấn qua một ngưỡng nhất định không
        // Ví dụ, sử dụng ngưỡng 0.1f để coi như là nhấn nút
        return leftTrigger > 0.1f || rightTrigger > 0.1f;
    }
}
