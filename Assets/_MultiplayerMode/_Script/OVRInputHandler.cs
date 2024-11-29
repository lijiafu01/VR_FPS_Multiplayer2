using UnityEngine;
namespace multiplayerMode
{
    public class OVRInputHandler : MonoBehaviour
    {
        private void Update()
        {
            OVRInputState.Instance.TriggerPressed = GetTriggerPressed(); // Lấy giá trị của nút kích hoạt
            OVRInputState.Instance.AButtonPressed = IsButtonAPressed(); // Lấy giá trị của nút A
            OVRInputState.Instance.LeftTriggerPressed = GetTriggerLeftPressed(); // Lấy giá trị của nút kích hoạt bên trái    
        }
        public bool IsButtonAPressed()
        {
            // Kiểm tra trạng thái của nút A trên bộ điều khiển phải
            return OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch);
        }
        public bool GetTriggerPressed()
        {
            // Lấy giá trị nút trigger bên phải
            float rightTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
            return rightTrigger > 0.1f;
        }
        public bool GetTriggerLeftPressed()
        {
            // Lấy giá trị nút trigger bên trái
            float leftTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
            return leftTrigger > 0.1f;
        }
    }
}

