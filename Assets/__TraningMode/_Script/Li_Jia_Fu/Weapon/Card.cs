using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private float _ForceMultiplier = 2.0f;
    public GameObject cardPrefab; // Prefab của thẻ bài
    public Transform rightHandAnchor; // Vị trí của controller phải
    public float minimumVelocity = 0.5f; // Vận tốc tối thiểu để bắt đầu theo dõi
    private Vector3 previousVelocity = Vector3.zero;
    private bool isTracking = false;

    void Update()
    {
        // Lấy vận tốc hiện tại của controller phải
        Vector3 currentVelocity = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);

        // Kiểm tra nếu vận tốc hiện tại lớn hơn vận tốc trước đó và lớn hơn ngưỡng tối thiểu
        if (currentVelocity.magnitude > previousVelocity.magnitude && currentVelocity.magnitude > minimumVelocity)
        {
            isTracking = true;
        }

        // Khi vận tốc bắt đầu giảm sau khi đạt đỉnh
        if (isTracking && currentVelocity.magnitude < previousVelocity.magnitude)
        {
            // Sử dụng hướng của controller phải để kiểm tra hướng vung
            Vector3 controllerForward = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;
            Vector3 movementDirection = previousVelocity.normalized;

            // Tính tích vô hướng giữa hướng di chuyển và hướng phía trước của controller
            float dotProduct = Vector3.Dot(controllerForward, movementDirection);

            // Đặt ngưỡng cho góc tối đa chấp nhận
            float angleThreshold = 90f; // Góc tối đa chấp nhận (90 độ)
            float dotThreshold = Mathf.Cos(angleThreshold * Mathf.Deg2Rad); // Chuyển đổi sang radians

            if (dotProduct > dotThreshold)
            {
                // Hướng di chuyển trong phạm vi 180 độ phía trước của controller
                SpawnCard(previousVelocity);
            }

            isTracking = false;
        }

        // Cập nhật vận tốc trước đó cho lần kiểm tra tiếp theo
        previousVelocity = currentVelocity;
    }

    void SpawnCard(Vector3 initialVelocity)
    {
        // Tạo một instance của thẻ bài tại vị trí của controller phải
        GameObject cardInstance = Instantiate(cardPrefab, rightHandAnchor.position, rightHandAnchor.rotation);

        // Thêm Rigidbody nếu prefab chưa có
        Rigidbody rb = cardInstance.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = cardInstance.AddComponent<Rigidbody>();
        }

        // Đặt vận tốc ban đầu cho thẻ bài theo hướng phía trước của controller phải
        Vector3 throwDirection = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;
        rb.velocity = throwDirection.normalized * initialVelocity.magnitude * _ForceMultiplier; // Điều chỉnh hệ số nhân nếu cần
    }
}
