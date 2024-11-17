using multiplayerMode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float force = 500f; // Biến để điều chỉnh lực
    [SerializeField] private float angleOffset = 60f; // Biến để điều chỉnh góc nhích lên

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<HardwareRig>(out var hitHardwareRig))
        {
            // Tính toán hướng nhích lên angleOffset độ
            Vector3 direction = Quaternion.AngleAxis(-angleOffset, transform.right) * transform.forward;

            // Gọi hàm để áp dụng lực lên đối tượng
            hitHardwareRig.ReceiveImpact2(direction, force);
        }
    }
}
