using UnityEngine;

public class OVRInputState
{
    private static OVRInputState instance;

    // Đây là thuộc tính để truy cập thực thể singleton
    public static OVRInputState Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new OVRInputState();
            }
            return instance;
        }
    }

    // Đây là trạng thái nhấn của nút kích hoạt
    public bool TriggerPressed { get; set; }
    public bool AButtonPressed { get; set; }
    public bool LeftTriggerPressed { get; set; }
    // Constructor được đánh dấu là private để ngăn người dùng tạo thực thể mới từ bên ngoài
    private OVRInputState()
    {
    }
}
