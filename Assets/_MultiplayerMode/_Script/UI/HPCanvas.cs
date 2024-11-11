using UnityEngine;

public class HPCanvas : MonoBehaviour
{
    public Transform cameraTransform;
    public float distanceFromCamera = 5f; // Khoảng cách từ Canvas tới camera trên mặt phẳng XZ
    private float fixedY; // Tọa độ Y cố định của Canvas

    void Start()
    {
        if (cameraTransform == null)
        {
            // Tự động tìm camera chính nếu chưa gán
            cameraTransform = Camera.main.transform;
        }

        // Lưu lại tọa độ Y ban đầu của Canvas
        fixedY = transform.position.y;
    }

    void Update()
    {
        // Tính toán hướng từ camera tới phía trước trên mặt phẳng XZ
        Vector3 directionToCamera = cameraTransform.forward;
        directionToCamera.y = 0; // Loại bỏ thành phần Y để giữ nguyên chiều cao
        directionToCamera.Normalize();

        // Vị trí mới của Canvas
        Vector3 newPosition = cameraTransform.position + directionToCamera * distanceFromCamera;
        newPosition.y = fixedY; // Giữ nguyên chiều cao ban đầu

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
