using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BarrelFireNetWorked : NetworkBehaviour, IEnvironmentInteractable
{
    [SerializeField] private GameObject fireVFX;
    [Networked(OnChanged = nameof(OnFireStatusChanged))] // Theo dõi thay đổi của biến
    private bool isFire { get; set; } // Biến được đồng bộ hóa

    public override void Spawned()
    {
        if(isFire)
        {
            if (fireVFX != null)
            {
                if (!Object.HasStateAuthority) return;
                // Bật hiệu ứng lửa
                fireVFX.SetActive(true);
            }
        }
    }
    // Hàm này sẽ được gọi khi đối tượng bị bắn
    public void TriggerFire()
    {
        if (!isFire)
        {
            isFire = true; // Cập nhật biến đồng bộ, kích hoạt lửa
        }
    }

    // Hàm được gọi tự động khi `isFire` thay đổi
    private static void OnFireStatusChanged(Changed<BarrelFireNetWorked> changed)
    {
        changed.Behaviour.UpdateFireVFX();
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
    }

    public void OnHitByWeapon()
    {
        if(isFire) return;
        isFire = true;
    }
}
