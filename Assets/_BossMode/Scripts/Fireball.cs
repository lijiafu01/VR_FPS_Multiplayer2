using UnityEngine;
using Fusion;

public class Fireball : NetworkBehaviour
{
    [Networked]
    public Vector3 networkedLaunchVelocity { get; set; }

    public NetworkPrefabRef explosionPrefab; // Tham chiếu đến prefab vụ nổ
    public float explosionOffsetY = 0.5f; // Giá trị offset cho trục Y

    private Rigidbody rb;

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody>();

        // Áp dụng vận tốc đã được đồng bộ
        rb.velocity = networkedLaunchVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Object.HasStateAuthority)
        {
            // Tính toán vị trí vụ nổ với offset
            Vector3 explosionPosition = transform.position;
            explosionPosition.y += explosionOffsetY;

            // Sinh ra hiệu ứng vụ nổ tại vị trí đã điều chỉnh
            Runner.Spawn(explosionPrefab, explosionPosition, Quaternion.identity);

            // Hủy đối tượng quả cầu lửa
            Runner.Despawn(Object);
        }
    }
}
