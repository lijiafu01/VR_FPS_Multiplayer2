using UnityEngine;
using Fusion;

public class Fireball : NetworkBehaviour
{
    public float launchForce = 10f;

    private Rigidbody rb;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();

        // Áp dụng lực đẩy theo hướng mong muốn
        Vector3 launchDirection = transform.forward + transform.up * 0.5f; // Điều chỉnh hướng bay
        rb.AddForce(launchDirection.normalized * launchForce, ForceMode.Impulse);
    }
}
