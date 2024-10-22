using UnityEngine;

public class FireballNetworked : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;

    private Vector3 targetPosition;
    private Rigidbody rb;

    private float lifeTime = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 targetPos)
    {
        targetPosition = targetPos;

        // Tính toán vận tốc ban đầu
        CalculateInitialVelocity();

        // Bắt đầu đếm thời gian sống
        Destroy(gameObject, lifeTime);
    }

    void CalculateInitialVelocity()
    {
        Vector3 startPosition = transform.position;
        Vector3 direction = (targetPosition - startPosition).normalized;

        // Tính toán vận tốc ban đầu
        rb.velocity = direction * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Boss") return;

        // Gọi phương thức Explode để xử lý va chạm
        Explode();
    }

    [SerializeField]
    private GameObject explosionPrefab;

    void Explode()
    {
        // Tạo hiệu ứng nổ
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Hủy đối tượng Fireball
        Destroy(gameObject);
    }
}
