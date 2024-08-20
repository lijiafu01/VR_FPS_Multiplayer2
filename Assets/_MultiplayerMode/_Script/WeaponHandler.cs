using Fusion;
using UnityEngine;

public class WeaponHandler : NetworkBehaviour
{
    [SerializeField]
    private int damage = 10;
    [SerializeField]
    private Transform _shootPoint;

    [SerializeField]
    private Transform _leftShootPoint;

    [SerializeField]
    private LayerMask _hitLayer;

    [SerializeField]
    private HitOptions _hitOptions = HitOptions.IncludePhysX | HitOptions.SubtickAccuracy | HitOptions.IgnoreInputAuthority;

    public void Fire(bool isRight)
    {
        if (isRight)
        {
            // Vẽ một tia ray trong Scene view
            Debug.DrawRay(_shootPoint.position, _shootPoint.forward * 1000f, Color.red, 2.0f);
            
            if (Runner.LagCompensation.Raycast(_shootPoint.position,
                    _shootPoint.forward,
                    Mathf.Infinity,
                    Object.InputAuthority,
                    out LagCompensatedHit hit,
                    _hitLayer,
                    _hitOptions))
            {
                Debug.Log($"Hit {hit.GameObject.name} at {hit.Point}");

                if (hit.GameObject.TryGetComponent<PlayerController>(out var hitPlayerController))
                {
                    string playerName = GameManager.Instance.PlayerData.playerName;
                    // Sử dụng RPC để thông báo cho client của người bị bắn gọi hàm TakeDamage và truyền vị trí va chạm
                    hitPlayerController.TakeDamage_RPC(damage, hit.Point, hit.Normal, playerName);
                }

                // Vẽ đường từ điểm bắn đến điểm va chạm
                Debug.DrawLine(_shootPoint.position, hit.Point, Color.green, 2.0f);
            }
        }
        else
        {
            // Vẽ một tia ray trong Scene view
            Debug.DrawRay(_leftShootPoint.position, _leftShootPoint.forward * 1000f, Color.red, 2.0f);

            if (Runner.LagCompensation.Raycast(_leftShootPoint.position,
                    _leftShootPoint.forward,
                    Mathf.Infinity,
                    Object.InputAuthority,
                    out LagCompensatedHit hit,
                    _hitLayer,
                    _hitOptions))
            {
                Debug.Log($"Hit {hit.GameObject.name} at {hit.Point}");

                if (hit.GameObject.TryGetComponent<PlayerController>(out var hitPlayerController))
                {
                    string playerName = GameManager.Instance.PlayerData.playerName;
                    // Sử dụng RPC để thông báo cho client của người bị bắn gọi hàm TakeDamage và truyền vị trí va chạm
                    hitPlayerController.TakeDamage_RPC(damage, hit.Point, hit.Normal,playerName);
                }

                // Vẽ đường từ điểm bắn đến điểm va chạm
                Debug.DrawLine(_leftShootPoint.position, hit.Point, Color.green, 2.0f);
            }
        }
        
    }

}
