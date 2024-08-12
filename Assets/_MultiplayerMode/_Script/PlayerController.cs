using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private Transform shotPoint;
    [SerializeField]
    private Bullet bulletPrefab;
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (Object.HasStateAuthority)
        {
            ProcessInputs();
        }
    }

    private void ProcessInputs()
    {
        if (GetInput(out InputData inputData))
        {
            // Thực hiện hành động dựa trên nút nhấn
            if (inputData.Button.IsSet(InputButton.Fire))
            {
                FireWeapon();
            }

            // Xử lý thêm các đầu vào khác tại đây
        }
    }

    private void FireWeapon()
    {
        Debug.Log("dev_Fire");
        // Vị trí bắn đạn được điều chỉnh để phù hợp hơn với vị trí của controller
        Vector3 spawnPosition = shotPoint.position + shotPoint.forward * 0.1f;

        // Sử dụng cả vector 'up' hiện tại của controller để đảm bảo rằng đạn sẽ bay theo đúng hướng
        // kể cả khi controller được nghiêng lên hoặc xuống
        Quaternion spawnRotation = Quaternion.LookRotation(shotPoint.forward, shotPoint.up);

        Runner.Spawn(bulletPrefab, spawnPosition, spawnRotation, Object.InputAuthority);
    }
}
