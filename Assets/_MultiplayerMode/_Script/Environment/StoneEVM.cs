using UnityEngine;
public class StoneEVM : MonoBehaviour, IstatisEvm
{
    public GameObject evmVFX;
    public void OnHitByWeapon()
    {
    }
    public void StaticEVM(Vector3 spawnPoint, Vector3 hitNormal)
    {
        // Tạo hiệu ứng với hướng đúng theo pháp tuyến va chạm
        GameObject gameObject = Instantiate(evmVFX, spawnPoint, Quaternion.LookRotation(hitNormal));
        Destroy(gameObject, 1.2f);
    }

}
