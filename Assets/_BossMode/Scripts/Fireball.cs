using UnityEngine;

public class Fireball : MonoBehaviour
{
    public ThrowBallSkill ThrowBallSkill = null;
    [SerializeField]
    private GameObject explosionPrefab;
    private Vector3 initialVelocity;
    private Vector3 targetPosition;
    private Rigidbody rb;
    private bool hasInitializedVelocity = false;

    [SerializeField]
    private float lifeTime = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Bắt đầu đếm thời gian sống
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(Vector3 targetPos)
    {
        targetPosition = targetPos;

        // Tính toán vận tốc ban đầu
        CalculateInitialVelocity();
    }

    void CalculateInitialVelocity()
    {
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
        initialVelocity = direction * initialSpeed * Mathf.Cos(45 * Mathf.Deg2Rad) + Vector3.up * initialSpeed * Mathf.Sin(45 * Mathf.Deg2Rad);
    }

    void FixedUpdate()
    {
        // Áp dụng vận tốc ban đầu nếu chưa thực hiện
        if (!hasInitializedVelocity && initialVelocity != Vector3.zero)
        {
            rb.velocity = initialVelocity;
            hasInitializedVelocity = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("bosskill1_ ");
        if (collision.gameObject.tag == "Boss") return;

        if(ThrowBallSkill != null)
        {
            Debug.Log("bosskill1_ 2");

            // Lấy vị trí va chạm
            Vector3 collisionPosition = collision.GetContact(0).point;

            // Lấy pháp tuyến va chạm
            Vector3 collisionNormal = collision.GetContact(0).normal;
            // Gọi phương thức Explode để xử lý va chạm
            ThrowBallSkill.Explode(collisionPosition, collisionNormal);
        }
       Destroy(gameObject);

    }
}
