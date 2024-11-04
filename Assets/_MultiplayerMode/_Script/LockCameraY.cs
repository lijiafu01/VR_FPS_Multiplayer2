using UnityEngine;

public class LockCameraY : MonoBehaviour
{
    private float fixedY; // Giá trị Y cố định

    private void Start()
    {
        fixedY = transform.position.y; // Lưu vị trí Y ban đầu
    }

    private void Update()
    {
        // Giữ nguyên vị trí Y trong khi cho phép các trục X và Z thay đổi
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);
    }
}
