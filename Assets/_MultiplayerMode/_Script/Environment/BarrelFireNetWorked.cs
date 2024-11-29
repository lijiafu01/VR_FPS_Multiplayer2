using Fusion;
using System.Collections;
using UnityEngine;
public class BarrelFireNetWorked : NetworkBehaviour, IEnvironmentInteractable
{
    [SerializeField] AudioSource audioFX;
    [SerializeField] AudioSource audio2FX;
    [SerializeField] private GameObject fireVFX;
    [Networked]
    private bool isFire { get; set; } // Biến được đồng bộ hóa
    public override void Spawned()
    {
        if(isFire)
        {
            if (fireVFX != null)
            {
                fireVFX.SetActive(true);
                audioFX.Play();
                audio2FX.Play();
            }
        }
    }
    public void StaticEVM(Vector3 spawnPoint, Vector3 hitNormal)
    {
    }
    // Hàm này sẽ được gọi khi đối tượng bị bắn
    public void TriggerFire()
    {
        if (!isFire)
        {
            isFire = true; // Cập nhật biến đồng bộ, kích hoạt lửa
        }
    }
    // Hàm cập nhật trạng thái của hiệu ứng lửa
    private void UpdateFireVFX()
    {
        if (isFire)
        {
            if (fireVFX != null)
            {
                // Bật hiệu ứng lửa
                fireVFX.SetActive(true);
                audioFX.Play();
                audio2FX.Play();
                // Bắt đầu coroutine để tắt lửa sau 30 giây
                StartCoroutine(DisableFireAfterTime(30f));
            }
            else
            {
                Debug.LogError("Fire VFX is not assigned.");
            }
        }
        else
        {
            if (fireVFX != null)
            {
                // Tắt hiệu ứng lửa
                fireVFX.SetActive(false);
                audioFX.Stop();
            }
        }
    }
    // Coroutine để tắt hiệu ứng sau thời gian nhất định
    private IEnumerator DisableFireAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!Object.HasStateAuthority) yield break;

        // Đặt lại biến `isFire` để tắt hiệu ứng lửa
        isFire = false;
        SetSFX_RPC(isFire);
    }
    public void OnHitByWeapon()
    {
        if(isFire) return;
        isFire = true;
        SetSFX_RPC(isFire);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    void SetSFX_RPC(bool isfire)
    {
        isFire = isfire;
        UpdateFireVFX();
    }
}
