using Fusion;
using UnityEngine;

public class FireballNetworked : NetworkBehaviour
{
    [Networked]
    private Vector3 initialVelocity { get; set; }

    private TickTimer lifeTimer;

    [Networked]
    private Vector3 targetPosition { get; set; }

    [SerializeField]
    private float speed = 10f;

    private Rigidbody rb;
    [Networked]
    private NetworkBool hasInitializedVelocity { get; set; }

    public override void Spawned()
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, 5f);
        rb = GetComponent<Rigidbody>();

        /*if (Object.HasStateAuthority)
        {
            // Áp dụng vận tốc ban đầu cho Rigidbody
            rb.velocity = initialVelocity;
        }*/
    }


    public void SetTargetPosition(Vector3 position)
    {
        if (Object.HasStateAuthority)
        {
            targetPosition = position;

            // Tính toán khoảng cách ngang trên mặt phẳng XZ
            Vector3 startPosition = transform.position;
            Vector3 horizontalDisplacement = new Vector3(targetPosition.x - startPosition.x, 0, targetPosition.z - startPosition.z);
            float distance = horizontalDisplacement.magnitude;

            // Gia tốc trọng trường
            float gravity = Mathf.Abs(Physics.gravity.y);

            // Tính toán vận tốc ban đầu cần thiết
            float initialSpeed = Mathf.Sqrt(distance * gravity);

            // Tính toán hướng vận tốc ban đầu
            Vector3 direction = horizontalDisplacement.normalized;

            // Tính toán vector vận tốc ban đầu ở góc 45 độ
            Vector3 velocity = direction * initialSpeed * Mathf.Cos(45 * Mathf.Deg2Rad) + Vector3.up * initialSpeed * Mathf.Sin(45 * Mathf.Deg2Rad);

            // Gán cho biến mạng
            initialVelocity = velocity;
        }
    }


    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            // Áp dụng vận tốc ban đầu nếu chưa thực hiện
            if (!hasInitializedVelocity && initialVelocity != Vector3.zero)
            {
                rb.velocity = initialVelocity;
                hasInitializedVelocity = true;
            }
        }
       

        if (lifeTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Boss") return;
        if (Object.HasStateAuthority)
        {

            // Gọi phương thức Explode để xử lý va chạm
            Explode();
        }
    }
    [SerializeField] private NetworkPrefabRef explosionPrefabNetworked;
    void Explode()
    {
        // Sinh vụ nổ trên State Authority
        if (Object.HasStateAuthority)
        {
            Runner.Spawn(explosionPrefabNetworked, transform.position, Quaternion.identity, Object.InputAuthority);
        }
        Runner.Despawn(Object);
    }
}
