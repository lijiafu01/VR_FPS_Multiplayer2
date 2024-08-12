using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkRig : NetworkBehaviour
{
    [SerializeField]
    private Transform shotPoint;
    [SerializeField]
    private Bullet bulletPrefab;
    // Xác định xem đây có phải là bản sao cục bộ hay không
    public bool IsLocalNetworkRig => Object.HasStateAuthority;

    [Header("RigComponents")]
    [SerializeField]
    private NetworkTransform playerTransform;

    [SerializeField]
    private NetworkTransform headTransform;

    [SerializeField]
    private NetworkTransform leftHandTransform;

    [SerializeField]
    private NetworkTransform rightHandTransform;

    HardwareRig hardwareRig;
    private NetworkButtons _previousButton { get; set; }
    public override void Spawned()
    {
        base.Spawned();

        // Nếu là bản sao cục bộ, tìm kiếm và gán HardwareRig
        if (IsLocalNetworkRig)
        {
            hardwareRig = FindObjectOfType<HardwareRig>();
            if (hardwareRig == null)
                Debug.LogError("Missing HardwareRig in the scene");
        }
        // else nó là một client
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        // Nhận dữ liệu đầu vào mạng và cập nhật trạng thái cho từng phần tử
        if (GetInput<RigState>(out var input))
        {
            playerTransform.transform.SetPositionAndRotation(input.PlayerPosition, input.PlayerRotation);
            headTransform.transform.SetPositionAndRotation(input.HeadsetPosition, input.HeadsetRotation);
            leftHandTransform.transform.SetPositionAndRotation(input.LeftHandPosition, input.LeftHandRotation);
            rightHandTransform.transform.SetPositionAndRotation(input.RightHandPosition, input.RightHandRotation);

            var buttonPressed = input.Button.GetPressed(_previousButton);
            _previousButton = input.Button;


            if (buttonPressed.IsSet(InputButton.Fire))
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
          
        /*if (IsLocalNetworkRig)
        {
            Debug.Log("dev_Fire1");
            ProcessInputs();
        }*/
        /*if (GetInput(out InputData data))
        {

            var buttonPressed = data.Button.GetPressed(_previousButton);
            _previousButton = data.Button;


            if (buttonPressed.IsSet(InputButton.Fire))
            {
                Debug.Log("dev_Fire");
                // Vị trí bắn đạn được điều chỉnh để phù hợp hơn với vị trí của controller
                Vector3 spawnPosition = shotPoint.position + shotPoint.forward * 0.1f;

                // Sử dụng cả vector 'up' hiện tại của controller để đảm bảo rằng đạn sẽ bay theo đúng hướng
                // kể cả khi controller được nghiêng lên hoặc xuống
                Quaternion spawnRotation = Quaternion.LookRotation(shotPoint.forward, shotPoint.up);

                Runner.Spawn(bulletPrefab, spawnPosition, spawnRotation, Object.InputAuthority);
            }

        }*/
    }
   /* private void ProcessInputs()
    {
        if (GetInput(out InputData data))
        {
            var buttonPressed = data.Button.GetPressed(_previousButton);
            _previousButton = data.Button;

            
            if (buttonPressed.IsSet(InputButton.Fire))
            {
                FireWeapon();
            }
        }

    }*/
   /* private void FireWeapon()
    {
        Debug.Log("dev_Fire4");
        // Vị trí bắn đạn được điều chỉnh để phù hợp hơn với vị trí của controller
        Vector3 spawnPosition = shotPoint.position + shotPoint.forward * 0.1f;

        // Sử dụng cả vector 'up' hiện tại của controller để đảm bảo rằng đạn sẽ bay theo đúng hướng
        // kể cả khi controller được nghiêng lên hoặc xuống
        Quaternion spawnRotation = Quaternion.LookRotation(shotPoint.forward, shotPoint.up);

        Runner.Spawn(bulletPrefab, spawnPosition, spawnRotation, Object.InputAuthority);
    }*/
    public override void Render()
    {
        base.Render();
        // Nếu là bản sao cục bộ, sử dụng thông tin từ HardwareRig để cập nhật vị trí và hướng
        if (IsLocalNetworkRig)
        {
            playerTransform.InterpolationTarget.SetPositionAndRotation(hardwareRig.playerTransform.position, hardwareRig.playerTransform.rotation);
            headTransform.InterpolationTarget.SetPositionAndRotation(hardwareRig.headTransform.position, hardwareRig.headTransform.rotation);
            leftHandTransform.InterpolationTarget.SetPositionAndRotation(hardwareRig.leftHandTransform.position, hardwareRig.leftHandTransform.rotation);
            rightHandTransform.InterpolationTarget.SetPositionAndRotation(hardwareRig.rightHandTransform.position, hardwareRig.rightHandTransform.rotation);
        }
    }
}
