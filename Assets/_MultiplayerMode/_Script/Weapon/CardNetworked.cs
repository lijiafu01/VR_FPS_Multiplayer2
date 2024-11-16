using Fusion;
using UnityEngine;
using UnityEngine.Windows;

namespace multiplayerMode
{
    public enum ControllerHand
    {
        Left,
        Right
    }

    public class CardNetworked : NetworkBehaviour
    {
        [SerializeField] private GameObject _CardModel;
        [SerializeField] private AudioSource audioSource;

        [SerializeField] private float maxForce = 10.0f;        // Lực đẩy tối đa
        [SerializeField] private float forceMultiplier = 2.0f;  // Hệ số nhân lực đẩy

        public NetworkObject cardPrefab;       // Prefab của thẻ bài
        public Transform leftHandAnchor;    // Vị trí của controller trái
        public Transform rightHandAnchor;   // Vị trí của controller phải

        private Transform handAnchor;       // Sẽ được gán là leftHandAnchor hoặc rightHandAnchor

        [SerializeField] private float minimumVelocity = 0.5f;  // Vận tốc tối thiểu để bắt đầu theo dõi
        private Vector3 previousVelocity = Vector3.zero;
        private bool isTracking = false;

        [SerializeField]
        private ControllerHand controllerHand = ControllerHand.Right; // Chọn controller trong Inspector

        private OVRInput.Controller selectedController;
        [SerializeField] private int damage;
        void SetupAttribute()
        {
            if (Object.HasStateAuthority)
            {
                if (PlayFabManager.Instance.UserData.PlayerAttributes.TryGetValue(UserEquipmentData.Instance.CurrentModelId, out AttributeData heroAttributes))
                {
                    HeroAttributeData heroAttributeData = PlayFabManager.Instance.UserData.GetHeroAttributeData();
                    if (heroAttributeData != null)
                    {
                        damage = heroAttributeData.Damage;
                    }
                }
            }

        }
       
        public override void Spawned()
        {
            SetupAttribute();
            if (!Object.HasInputAuthority) return;

            if (controllerHand == ControllerHand.Left)
            {
                handAnchor = leftHandAnchor;
                selectedController = OVRInput.Controller.LTouch;
            }
            else
            {
                handAnchor = rightHandAnchor;
                selectedController = OVRInput.Controller.RTouch;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority) return;
            base.FixedUpdateNetwork();
            /*Vector3 currentVelocity = new Vector3();
            // Kiểm tra input từ hệ thống mạng
            if (GetInput<RigState>(out var input))
            {
                currentVelocity = input.ForceControllerLeft;
            }*/
           

            // Lấy vận tốc hiện tại của controller được chọn
           Vector3 currentVelocity = OVRInput.GetLocalControllerVelocity(selectedController);

            // Kiểm tra nếu vận tốc hiện tại lớn hơn vận tốc trước đó và lớn hơn ngưỡng tối thiểu
            if (currentVelocity.magnitude > previousVelocity.magnitude && currentVelocity.magnitude > minimumVelocity)
            {
                isTracking = true;
            }

            // Khi vận tốc bắt đầu giảm sau khi đạt đỉnh
            if (isTracking && currentVelocity.magnitude < previousVelocity.magnitude)
            {
                // Sử dụng hướng của controller để kiểm tra hướng vung
                Vector3 controllerForward = OVRInput.GetLocalControllerRotation(selectedController) * Vector3.forward;
                Vector3 movementDirection = previousVelocity.normalized;

                // Tính tích vô hướng giữa hướng di chuyển và hướng phía trước của controller
                float dotProduct = Vector3.Dot(controllerForward, movementDirection);

                // Đặt ngưỡng cho góc tối đa chấp nhận
                float angleThreshold = 90f; // Góc tối đa chấp nhận (90 độ)
                float dotThreshold = Mathf.Cos(angleThreshold * Mathf.Deg2Rad); // Chuyển đổi sang radians

                if (dotProduct > dotThreshold)
                {
                    
                    // Hướng di chuyển trong phạm vi 180 độ phía trước của controller
                    SpawnCard(previousVelocity.magnitude);
                }

                isTracking = false;
            }

            // Cập nhật vận tốc trước đó cho lần kiểm tra tiếp theo
            previousVelocity = currentVelocity;
        }

        bool _isShooting = false;

        void SpawnCard(float swingForce)
        {
            if (_isShooting) return;
            _CardModel.SetActive(false);
            _isShooting = true;
            Invoke("SetShootActive", 0.5f);
            audioSource.Play();
            NetworkObject cardInstance = Runner.Spawn(cardPrefab, handAnchor.position, handAnchor.rotation);
            CardProjectileNetworked cardProjectileNetworked = cardInstance.GetComponent<CardProjectileNetworked>();
            cardProjectileNetworked.Init(damage);
            // Tạo một instance của thẻ bài tại vị trí của controller
            //GameObject cardInstance = Instantiate(cardPrefab, handAnchor.position, handAnchor.rotation);

            // Thêm Rigidbody nếu prefab chưa có
            Rigidbody rb = cardInstance.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = cardInstance.gameObject.AddComponent<Rigidbody>();
            }

            // Tính toán lực đẩy, áp dụng hệ số nhân và giới hạn bởi lực tối đa
            float force = Mathf.Min(swingForce * forceMultiplier, maxForce);

            // Đặt vận tốc ban đầu cho thẻ bài theo hướng phía trước của controller
            //Vector3 throwDirection = OVRInput.GetLocalControllerRotation(selectedController) * Vector3.forward;
            Vector3 throwDirection = handAnchor.forward;
            // Sử dụng AddForce với ForceMode.Impulse để áp dụng lực tức thời
            rb.AddForce(throwDirection.normalized * force, ForceMode.Impulse);
        }

        void SetShootActive()
        {
            _CardModel.SetActive(true);
            _isShooting = false;
        }
    }
}
