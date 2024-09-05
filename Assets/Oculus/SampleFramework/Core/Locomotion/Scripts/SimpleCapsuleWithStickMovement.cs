using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using multiplayerMode;
public class SimpleCapsuleWithStickMovement : MonoBehaviour
{
    public float JumpForce = 5.0f; // Lực nhảy, có thể tùy chỉnh trong Inspector
    private bool canJump = true; // Kiểm soát xem người chơi có thể nhảy hay không

    public bool EnableLinearMovement = true;
    public bool EnableRotation = true;
    public bool HMDRotatesPlayer = true;
    public bool RotationEitherThumbstick = false;
    public float RotationAngle = 45.0f;
    public float Speed = 0.0f;
    public OVRCameraRig CameraRig;

    private bool ReadyToSnapTurn;
    private Rigidbody _rigidbody;

    public event Action CameraUpdated;
    public event Action PreCharacterMove;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (CameraRig == null) CameraRig = GetComponentInChildren<OVRCameraRig>();
    }

    void Start()
    {
    }

    private void FixedUpdate()
    {
        if (CameraUpdated != null) CameraUpdated();
        if (PreCharacterMove != null) PreCharacterMove();

        if (HMDRotatesPlayer) RotatePlayerToHMD();
        if (EnableLinearMovement) StickMovement();
        if (EnableRotation) SnapTurn();

        if(OVRInputState.Instance.AButtonPressed)
        {
            PlayerJump();
        }
    }

    private void PlayerJump()
    {
        if (canJump)
        {
            _rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange); // Thêm lực nhảy
            canJump = false; // Sau khi nhảy, đặt lại không thể nhảy
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground")) // Kiểm tra va chạm với đối tượng được tag là Ground
        {
            Debug.Log("dev_co va cham voi Ground");
            canJump = true; // Nếu chạm đất, người chơi có thể nhảy lại
        }
    }

    void RotatePlayerToHMD()
    {
        Transform root = CameraRig.trackingSpace;
        Transform centerEye = CameraRig.centerEyeAnchor;

        Vector3 prevPos = root.position;
        Quaternion prevRot = root.rotation;

        transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);

        root.position = prevPos;
        root.rotation = prevRot;
    }

    void StickMovement()
    {
        Quaternion ort = CameraRig.centerEyeAnchor.rotation;
        Vector3 ortEuler = ort.eulerAngles;
        ortEuler.z = ortEuler.x = 0f;
        ort = Quaternion.Euler(ortEuler);

        Vector3 moveDir = Vector3.zero;
        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        moveDir += ort * (primaryAxis.x * Vector3.right);
        moveDir += ort * (primaryAxis.y * Vector3.forward);
        //_rigidbody.MovePosition(_rigidbody.transform.position + moveDir * Speed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(_rigidbody.position + moveDir * Speed * Time.fixedDeltaTime);
    }

    void SnapTurn()
    {
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickLeft) ||
            (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickLeft)))
        {
            if (ReadyToSnapTurn)
            {
                ReadyToSnapTurn = false;
                transform.RotateAround(CameraRig.centerEyeAnchor.position, Vector3.up, -RotationAngle);
            }
        }
        else if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickRight) ||
                 (RotationEitherThumbstick && OVRInput.Get(OVRInput.Button.PrimaryThumbstickRight)))
        {
            if (ReadyToSnapTurn)
            {
                ReadyToSnapTurn = false;
                transform.RotateAround(CameraRig.centerEyeAnchor.position, Vector3.up, RotationAngle);
            }
        }
        else
        {
            ReadyToSnapTurn = true;
        }
    }

}
