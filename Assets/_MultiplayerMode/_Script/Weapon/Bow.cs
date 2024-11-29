using UnityEngine;
using Fusion;
namespace multiplayerMode
{
    public class Bow : NetworkBehaviour
    {
        private NetworkButtons _previousButton { get; set; }
        [SerializeField] private Transform bowHandle;       // Điểm nắm giữa dây cung
        [SerializeField] private GameObject arrowPrefab;    // Prefab của mũi tên
        [SerializeField] private LineRenderer bowString;    // LineRenderer để vẽ dây cung
        [SerializeField] private float maxPullDistance = -0.8f; // Khoảng cách kéo tối đa, giá trị âm
        [SerializeField] private float pullStrengthMultiplier = 1500f; // Nhân số để tính lực bắn dựa trên độ kéo
        [SerializeField] private AudioSource drawSound;    // Âm thanh khi kéo dây cung
        private GameObject currentArrow;
        private bool isStringPulled = false;
        private Transform RightHand; // Vị trí của controller tay phải
        public Transform attackTransform; // Đảm bảo rằng bạn đã gán đúng Transform này trong Unity Editor
        public Transform bowstringCenter;
        private bool hasLeftCollider = false; // Biến để theo dõi khi tay rời khỏi collider
        [Networked] private bool isPulled { get; set; }
        protected void Start()
        {
            ResetString();
        }
        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            // Kiểm tra input từ hệ thống mạng
            if (GetInput<RigState>(out var input))
            {
                // Kiểm tra trạng thái nút Bow (PrimaryHandTrigger) nhấn giữ liên tục
                if (input.Button.IsSet(InputButton.Bow))  // Sử dụng IsSet thay cho GetPressed
                {
                    isPulled = true;  // Nút đang được nhấn giữ
                }
                else
                {
                    isPulled = false; // Nút không được nhấn giữ
                }
                // Cập nhật trạng thái nút trước đó
                _previousButton = input.Button;
            }
            // Kiểm tra xem tay đã rời khỏi collider và người chơi đã thả nút
            if (hasLeftCollider && !isPulled && isStringPulled)
            {
                ShootArrow();
                isStringPulled = false;
                hasLeftCollider = false; // Reset trạng thái sau khi bắn
            }
            // Kiểm tra khi tay phải đang kéo và nút Bow được giữ
            if (RightHand != null && isPulled)
            {
                if (!isStringPulled)
                {
                    if (Object.HasInputAuthority)
                    {
                        CreateArrow();
                    }
                    isStringPulled = true;
                    drawSound.Play();  // Phát âm thanh kéo dây cung
                }
                // Hướng kéo từ bowStringCenter đến attackTransform
                Vector3 pullDirection = (attackTransform.position - bowstringCenter.position).normalized;
                // Khoảng cách từ tay đến bowstringCenter
                float handDistance = Vector3.Distance(RightHand.position, bowstringCenter.position);
                // Giảm khoảng cách kéo nếu vượt quá giới hạn cho phép
                float pullDistance = Mathf.Max(0, Mathf.Min(handDistance, -maxPullDistance));
                // Cập nhật vị trí của bowHandle dọc theo hướng kéo
                bowHandle.position = bowstringCenter.position - pullDirection * pullDistance;
                // Cập nhật hướng của mũi tên để luôn hướng theo attackTransform
                if (currentArrow != null)
                {
                    currentArrow.transform.rotation = Quaternion.LookRotation(attackTransform.forward);
                }
            }
            else if (isStringPulled)
            {
                // Trường hợp khi tay vẫn kéo nhưng rời khỏi cung, và nút chưa thả
                if (hasLeftCollider)
                {
                }
                else
                {
                    // Khi thả dây cung
                    drawSound.Stop();
                    ShootArrow();
                    isStringPulled = false;
                    RightHand = null;
                }
            }
            else
            {
                ResetString();
            }
        }
        // Tạo mũi tên mới
        private void CreateArrow()
        {
            if (currentArrow == null)
            {
                // Kiểm tra arrowPrefab có được gán không
                if (arrowPrefab == null)
                {
                    Debug.LogError("dev16: arrowPrefab is not assigned!");
                    return;
                }
                // Tạo mũi tên mới và lấy đối tượng GameObject từ NetworkObject
                NetworkObject networkArrow = NetworkManager.Instance.Runner.Spawn(arrowPrefab, bowHandle.position, Quaternion.identity);
                if (networkArrow == null)
                {
                    Debug.LogError("dev16: Failed to spawn arrow with NetworkRunner.");
                    return;
                }
                currentArrow = networkArrow.gameObject;  // Lấy GameObject từ NetworkObject
                currentArrow.transform.SetParent(bowHandle);  // Đặt mũi tên là con của bowHandle
            }
        }
        // Xử lý khi tay phải chạm vào cung
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("RightHand"))
            {
                RightHand = other.transform;
                hasLeftCollider = false; // Reset khi tay vào lại collider
            }
            else
            {
            }
        }
        public AudioSource shootSFX;
        // Xử lý khi tay phải rời khỏi cung
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("RightHand"))
            {
                hasLeftCollider = true; // Đánh dấu rằng tay đã rời khỏi collider
            }
            else
            {
            }
            if (other.CompareTag("bullet"))
            {
                shootSFX.Play();
            }
        }
        // Bắn mũi tên
        private void ShootArrow()
        {
            if (currentArrow != null)
            {
                currentArrow.transform.SetParent(null);
                Rigidbody rb = currentArrow.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    Debug.LogError("dev16: Rigidbody is missing on arrow.");
                    return;
                }
                float pullDistance = Mathf.Abs(bowHandle.position.z - bowstringCenter.position.z);
                // Đảm bảo pullDistance không nhỏ hơn khoảng cách tối thiểu (ví dụ: 0.4)
                float minPullDistance = 0.2f; // Lực tối thiểu tương ứng với khoảng cách kéo -0.4
                pullDistance = Mathf.Max(pullDistance, minPullDistance);
                // Tính toán lực bắn mũi tên
                rb.AddForce(transform.forward * pullDistance * pullStrengthMultiplier);
                currentArrow = null;
                //shootSound.Play();  // Phát âm thanh bắn mũi tên
            }
            else
            {
                Debug.LogError("dev16: Attempted to shoot, but no arrow available.");
            }
        }
        // Đặt lại dây cung về vị trí ban đầu
        private void ResetString()
        {
            if (bowHandle == null || bowstringCenter == null)
            {
                Debug.LogError("dev16: bowHandle or bowstringCenter is not assigned.");
                return;
            }
            bowHandle.position = bowstringCenter.position;
        }
    }
}
