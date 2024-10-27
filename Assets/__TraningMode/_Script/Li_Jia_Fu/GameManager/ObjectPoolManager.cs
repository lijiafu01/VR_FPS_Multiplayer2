using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public string tag;          // Nhãn để nhận diện pool
    public GameObject prefab;   // Prefab của đối tượng trong pool
    public int size;            // Số lượng đối tượng ban đầu trong pool
}

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    public List<Pool> pools;    // Danh sách các pool được thiết lập trong Inspector

    // Từ điển lưu trữ các pool, với key là tag và value là danh sách các đối tượng trong pool
    private Dictionary<string, List<GameObject>> poolDictionary;

    // Từ điển lưu trữ các poolHolder (GameObject chứa các đối tượng của pool)
    private Dictionary<string, Transform> poolHolderDictionary;

    void Awake()
    {
        // Thiết lập Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Multiple instances of ObjectPoolManager detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        // Khởi tạo từ điển
        poolDictionary = new Dictionary<string, List<GameObject>>();
        poolHolderDictionary = new Dictionary<string, Transform>();

        // Tạo các pool
        foreach (Pool pool in pools)
        {
            // Tạo một GameObject để chứa các đối tượng của pool
            GameObject poolHolder = new GameObject(pool.tag + " Pool");
            poolHolder.transform.SetParent(transform);

            // Lưu trữ poolHolder vào từ điển
            poolHolderDictionary[pool.tag] = poolHolder.transform;

            // Tạo danh sách các đối tượng trong pool
            List<GameObject> objectPool = new List<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                // Tạo đối tượng và thêm vào pool
                GameObject obj = Instantiate(pool.prefab, Vector3.zero, Quaternion.identity);
                obj.transform.SetParent(poolHolder.transform);
                obj.SetActive(false);
                objectPool.Add(obj);
            }

            // Lưu trữ pool vào từ điển
            poolDictionary[pool.tag] = objectPool;
        }
    }

    // Phương thức mở rộng pool và tạo đối tượng mới
    private GameObject ExpandPoolAndGetObject(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        Pool pool = pools.Find(p => p.tag == tag);
        if (pool == null)
        {
            Debug.LogError("No pool definition found for tag " + tag);
            return null;
        }

        if (!poolHolderDictionary.TryGetValue(tag, out Transform poolHolder))
        {
            Debug.LogError("No pool holder found for tag " + tag);
            return null;
        }

        // Tạo đối tượng mới
        GameObject obj = Instantiate(pool.prefab, position, rotation);
        obj.transform.SetParent(poolHolder);
        obj.SetActive(false);

        // Thêm đối tượng mới vào pool
        poolDictionary[tag].Add(obj);

        return obj;
    }

    // Phương thức lấy đối tượng từ pool
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        // Tìm đối tượng chưa được sử dụng trong pool
        GameObject objectToSpawn = poolDictionary[tag].FirstOrDefault(obj => !obj.activeInHierarchy);

        if (objectToSpawn == null)
        {
            // Mở rộng pool nếu tất cả đối tượng đều đang được sử dụng
            objectToSpawn = ExpandPoolAndGetObject(tag, position, rotation);
            if (objectToSpawn == null)
            {
                Debug.LogError("Failed to expand pool and get new object for tag " + tag);
                return null;
            }
        }

        // Kích hoạt và thiết lập vị trí, hướng cho đối tượng
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        return objectToSpawn;
    }

    // Phương thức trả đối tượng về pool
    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return;
        }

        // Xử lý các đối tượng con (nếu có)
        foreach (Transform child in objectToReturn.transform)
        {
            string childTag = child.gameObject.tag;

            if (poolDictionary.ContainsKey(childTag))
            {
                if (poolHolderDictionary.TryGetValue(childTag, out Transform childPoolHolder))
                {
                    // Đặt đối tượng con về pool tương ứng
                    child.SetParent(childPoolHolder);
                    child.gameObject.SetActive(false);

                    // Reset các thuộc tính nếu cần
                    ResetObject(child.gameObject);
                }
                else
                {
                    Debug.LogWarning("No pool holder found for child tag " + childTag);
                }
            }
            else
            {
                Debug.LogWarning("No pool found for child tag " + childTag);
            }
        }

        // Reset các thuộc tính của đối tượng trả về pool
        ResetObject(objectToReturn);

        // Đặt lại đối tượng cha của objectToReturn
        if (poolHolderDictionary.TryGetValue(tag, out Transform parentPoolHolder))
        {
            objectToReturn.transform.SetParent(parentPoolHolder);
        }
        else
        {
            Debug.LogWarning("No pool holder found for tag " + tag);
        }

        objectToReturn.SetActive(false);
    }

    // Phương thức reset các thuộc tính của đối tượng
    private void ResetObject(GameObject obj)
    {
        // Reset vị trí và hướng
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;

        // Reset Rigidbody nếu có
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }

        // Reset Animator nếu có
        Animator animator = obj.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        // Reset ParticleSystem nếu có
        ParticleSystem ps = obj.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Clear();
            ps.Stop();
        }

        // Reset các thành phần khác nếu cần
        // Ví dụ: Health, trạng thái nội bộ của script, v.v.
    }
}
