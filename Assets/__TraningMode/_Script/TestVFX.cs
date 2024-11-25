using UnityEngine;

public class TestVFX : MonoBehaviour
{
    public Transform spawnPos;
    public GameObject flashVFX;
    public GameObject prefabToFire; // Prefab sẽ được bắn ra
    public float fireForce = 10f;    // Lực bắn của prefab

    void Update()
    {
        // Kiểm tra nếu phím A được nhấn xuống
        if (Input.GetKeyDown(KeyCode.A))
        {
            FirePrefab();
        }
    }

    void FirePrefab()
    {
        GameObject spawnedPrefab = Instantiate(prefabToFire, spawnPos.position, transform.rotation);

        // Lấy thành phần Rigidbody của prefab để áp dụng lực
        Rigidbody rb = spawnedPrefab.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Áp dụng lực theo trục Y địa phương của đối tượng giữ script
            Vector3 forceDirection = transform.up * fireForce;
            rb.AddForce(forceDirection, ForceMode.VelocityChange);
        }
        else
        {
            Debug.LogWarning("Prefab không có thành phần Rigidbody.");
        }
    }
}
