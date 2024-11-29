using Fusion;
using multiplayerMode;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LocalPlayer : MonoBehaviour
{
    [SerializeField] private int maxMana = 100; // Lượng mana tối đa
    [SerializeField] private int currentMana; // Lượng mana hiện tại
    [SerializeField] private int manaRegenRate = 50; // Lượng mana hồi mỗi 5 giây
    [SerializeField] private int manaCostPerJump = 10; // Lượng mana trừ mỗi lần nhảy
    [SerializeField] private float regenInterval = 5f; // Thời gian hồi mana (5 giây)
    [SerializeField] private Slider manaSlider; // Slider để hiển thị mana
    [SerializeField] private TextMeshProUGUI manaText; // Text để hiển thị lượng mana
    public NetworkObject jumpVFX;
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
    // Thêm biến cho lực nhảy và kiểm tra trạng thái đứng trên mặt đất
    public float jumpForce = 5f; // Lực nhảy
    private bool isGrounded = true; // Kiểm tra nhân vật đang đứng trên mặt đất
    private RigidbodyConstraints originalConstraints; // Lưu trạng thái constraints ban đầu
    public bool  isMainGame = false;
    private void UnlockConstraints()
    {
        // Mở khóa và phục hồi lại trạng thái constraints ban đầu
        _rigidbody.constraints = originalConstraints;
    }
    private void Awake()
    {
        if(SceneManager.GetActiveScene().name == "MainGame")
        {
            isMainGame = true;

        }
        _rigidbody = GetComponent<Rigidbody>();
        if (CameraRig == null) CameraRig = GetComponentInChildren<OVRCameraRig>();
        // Lấy thành phần Rigidbody
        _rigidbody = GetComponent<Rigidbody>();
        // Lưu lại trạng thái constraints ban đầu
        originalConstraints = _rigidbody.constraints;
        // Khóa tất cả các trục chuyển động và xoay
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        // Gọi hàm mở khóa sau 3 giây
        Invoke(nameof(UnlockConstraints), 3f);
    }
    void SetupAttribute()
    {
        if (PlayFabManager.Instance.UserData.PlayerAttributes.TryGetValue(UserEquipmentData.Instance.CurrentModelId, out AttributeData heroAttributes))
        {
            HeroAttributeData heroAttributeData = PlayFabManager.Instance.UserData.GetHeroAttributeData();
            if (heroAttributeData != null)
            {
                Speed = heroAttributeData.Movespeed;
            }
        }
    }
    void Start()
    {
        SetupAttribute();
        currentMana = maxMana;
        UpdateManaUI();
        InvokeRepeating(nameof(RegenerateMana), regenInterval, regenInterval);
    }
    private void RegenerateMana()
    {
        if (currentMana < maxMana)
        {
            currentMana = Mathf.Min(currentMana + manaRegenRate, maxMana);
            UpdateManaUI();
        }
    }
    private void UpdateManaUI()
    {
        if (manaSlider != null)
        {
            manaSlider.value = (float)currentMana / maxMana;
        }
        if (manaText != null)
        {
            manaText.text = $"{currentMana}/{maxMana}";
        }
    }
    private bool jumpRequested = false;
    private void Update()
    {
        // Kiểm tra nếu người chơi nhấn nút nhảy
        if ((OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.V)) && isMainGame)
        {
            jumpRequested = true;
        }
    }
    private void FixedUpdate()
    {
        if (CameraUpdated != null) CameraUpdated();
        if (PreCharacterMove != null) PreCharacterMove();

        if (HMDRotatesPlayer) RotatePlayerToHMD();
        // Test PC movement
        HandlePCInput();
        if (EnableLinearMovement) StickMovement();
        if (EnableRotation) SnapTurn();
        // Nếu đã yêu cầu nhảy, thực hiện lệnh nhảy và đặt lại trạng thái
        if (jumpRequested)
        {
            HandleJump();
            jumpRequested = false;
        }
    }
    void HandlePCInput()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        // Xử lý di chuyển trên PC bằng cách kiểm tra phím A, S, D, W
        Quaternion ort = CameraRig.centerEyeAnchor.rotation;
        Vector3 ortEuler = ort.eulerAngles;
        ortEuler.z = ortEuler.x = 0f;
        ort = Quaternion.Euler(ortEuler);
        Vector3 moveDir = Vector3.zero;
        // Kiểm tra phím được nhấn
        if (Input.GetKey(KeyCode.W))
        {
            moveDir += ort * Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDir += ort * Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDir += ort * Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDir += ort * Vector3.right;
        }
        // Chuẩn hóa vector di chuyển nếu có phím được nhấn
        if (moveDir != Vector3.zero)
        {
            moveDir = moveDir.normalized;
            _rigidbody.AddForce(moveDir * 15, ForceMode.Acceleration);
        }
        // Xử lý xoay bằng chuột trên PC
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(0, mouseX * RotationAngle * Time.fixedDeltaTime, 0);
#endif
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
    // Thêm hàm xử lý nhảy
    void HandleJump()
    {
        if (currentMana >= manaCostPerJump)
        {
            // Trừ mana và cập nhật UI
            currentMana -= manaCostPerJump;
            UpdateManaUI();
            // Đặt lại vận tốc của Rigidbody về 0 trên tất cả các trục trước khi nhảy
            _rigidbody.velocity = Vector3.zero;
            // Sinh hiệu ứng nhảy
            NetworkManager.Instance.Runner.Spawn(jumpVFX, transform.position, Quaternion.identity);
            // Áp dụng lực nhảy
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }
}
