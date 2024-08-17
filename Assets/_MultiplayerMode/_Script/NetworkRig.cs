using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Oculus.Platform;

public class NetworkRig : NetworkBehaviour
{
    [SerializeField]
    private GameObject _visuals;
    [SerializeField]
    private Transform shotPoint;
    [SerializeField]
    private Bullet bulletPrefab;
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

    [SerializeField]
    private WeaponHandler _weaponHandler;
    public override void Spawned()
    {
        base.Spawned();
        if (IsLocalNetworkRig)
        {
            hardwareRig = FindObjectOfType<HardwareRig>();
            if (hardwareRig == null)
                Debug.LogError("Missing HardwareRig in the scene");

            if (_visuals != null)
            {
                MeshRenderer[] renderers = _visuals.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer renderer in renderers)
                {
                    renderer.enabled = false;
                }
            }
        }
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        // Nhận dữ liệu đầu vào mạng và cập nhật trạng thái cho từng phần tử
        if (GetInput<RigState>(out var input))
        {
            playerTransform.transform.SetPositionAndRotation(input.PlayerPosition, input.PlayerRotation);

            // Xử lý riêng cho rotation của head chỉ trên trục Y
            Vector3 headPosition = input.HeadsetPosition;
            Quaternion headRotation = Quaternion.Euler(0, input.HeadsetRotation.eulerAngles.y, 0);

            headTransform.transform.SetPositionAndRotation(headPosition, headRotation);

            leftHandTransform.transform.SetPositionAndRotation(input.LeftHandPosition, input.LeftHandRotation);
            rightHandTransform.transform.SetPositionAndRotation(input.RightHandPosition, input.RightHandRotation);

            var buttonPressed = input.Button.GetPressed(_previousButton);
            _previousButton = input.Button;      
        }
    }
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
