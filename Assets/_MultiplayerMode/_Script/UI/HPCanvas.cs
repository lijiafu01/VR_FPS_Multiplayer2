using UnityEngine;

public class HPCanvas : MonoBehaviour
{
    public Transform cameraTransform;
    public float distanceFromCamera = 5f; // Khoảng cách từ Canvas tới camera trên mặt phẳng XZ
    private float verticalOffset = -1.4f; // Độ dịch chuyển thẳng đứng để đẩy Canvas xuống
    private float initialYOffset; // Độ chênh lệch Y giữa Canvas và camera

    void Start()
    {
        if (cameraTransform == null)
        {
            // Tự động tìm camera chính nếu chưa gán
            cameraTransform = Camera.main.transform;
        }

        // Lưu lại độ chênh lệch Y ban đầu giữa Canvas và camera
        initialYOffset = transform.position.y - cameraTransform.position.y;
    }

    void Update()
    {
        // Tính toán hướng từ camera tới phía trước trên mặt phẳng XZ
        Vector3 directionToCamera = cameraTransform.forward;
        directionToCamera.y = 0; // Loại bỏ thành phần Y để giữ nguyên chiều cao
        directionToCamera.Normalize();

        // Vị trí mới của Canvas
        Vector3 newPosition = cameraTransform.position + directionToCamera * distanceFromCamera;
        newPosition.y = cameraTransform.position.y + initialYOffset + verticalOffset; // Thêm độ dịch chuyển thẳng đứng

        // Cập nhật vị trí của Canvas
        transform.position = newPosition;

        // Xoay Canvas để đối diện với camera
        Vector3 lookDirection = transform.position - cameraTransform.position;
        lookDirection.y = 0; // Loại bỏ thành phần Y để tránh xoay theo trục Y

        if (lookDirection != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = rotation;
        }
    }
}
