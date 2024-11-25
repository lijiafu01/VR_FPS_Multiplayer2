using multiplayerMode;
using UnityEngine;
public class BossHPUI : MonoBehaviour
{
    private Transform localPlayerTransform;
    void Start()
    {
        localPlayerTransform = LocalManager.Instance.LocalPlayer.transform;
    }
    void Update()
    {
        if (localPlayerTransform != null)
        {
            transform.localRotation = Quaternion.identity;
            // Tính toán hướng từ UI đến người chơi
            Vector3 direction = localPlayerTransform.position - transform.position;
            direction.y = 0; // Chỉ xoay theo trục Y
            if (direction.sqrMagnitude > 0.001f)
            {
                // Tính toán góc xoay theo trục Y
                float targetYAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                // Tạo quaternion chỉ xoay theo trục Y
                Quaternion yRotation = Quaternion.Euler(0f, targetYAngle, 0f);
                // Áp dụng góc xoay
                transform.rotation = yRotation;
            }
        }
    }
}
