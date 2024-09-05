using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;
using multiplayerMode;
using ExitGames.Client.Photon;
using Fusion;
using UnityEngine.UIElements;
using ExitGames.Client.Photon.StructWrapping;

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
        [SerializeField] private AudioSource shootSound;   // Âm thanh khi bắn mũi tên

        private GameObject currentArrow;
        private bool isStringPulled = false;
        private Transform RightHand; // Vị trí của controller tay phải
        public Transform attackTransform; // Đảm bảo rằng bạn đã gán đúng Transform này trong Unity Editor
        public Transform bowstringCenter;
        //private bool isPulled = false;
        private bool hasLeftCollider = false; // Biến để theo dõi khi tay rời khỏi collider
        [Networked] private bool isPulled { get; set; }

        protected void Start()
        {
            ResetString();
            Debug.Log("dev16: Bow system initialized.");
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
                    Debug.Log("dev17: Button Bow is being held continuously.");
                }
                else
                {
                    isPulled = false; // Nút không được nhấn giữ
                    Debug.Log("dev17: Button Bow is not being held.");
                }

                // Cập nhật trạng thái nút trước đó
                _previousButton = input.Button;
            }

            // Kiểm tra xem tay đã rời khỏi collider và người chơi đã thả nút
            if (hasLeftCollider && !isPulled && isStringPulled)
            {
                Debug.Log("dev16: Player has left the collider and released the bow. Shooting arrow.");
                ShootArrow();
                isStringPulled = false;
                hasLeftCollider = false; // Reset trạng thái sau khi bắn
            }

            Debug.Log("dev16: RightHand is: " + (RightHand != null) + " | isPulled: " + isPulled);

            // Kiểm tra khi tay phải đang kéo và nút Bow được giữ
            if (RightHand != null && isPulled)
            {
                Debug.Log("dev16: RightHand is active and pulling.");

                if (!isStringPulled)
                {
                    Debug.Log("dev16: String is not yet pulled, creating arrow.");
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
                Debug.Log("dev16: Hand distance: " + handDistance);

                // Giảm khoảng cách kéo nếu vượt quá giới hạn cho phép
                float pullDistance = Mathf.Max(0, Mathf.Min(handDistance, -maxPullDistance));
                Debug.Log("dev16: Pull distance (limited by maxPullDistance): " + pullDistance);

                // Cập nhật vị trí của bowHandle dọc theo hướng kéo
                bowHandle.position = bowstringCenter.position - pullDirection * pullDistance;
                Debug.Log("dev16: BowHandle position updated.");

                // Cập nhật hướng của mũi tên để luôn hướng theo attackTransform
                if (currentArrow != null)
                {
                    currentArrow.transform.rotation = Quaternion.LookRotation(attackTransform.forward);
                    Debug.Log("dev16: Arrow rotation updated.");
                }
            }
            else if (isStringPulled)
            {
                // Trường hợp khi tay vẫn kéo nhưng rời khỏi cung, và nút chưa thả
                if (hasLeftCollider)
                {
                    Debug.Log("dev16: Hand left the bow, but the string is still pulled. Waiting for release.");
                }
                else
                {
                    // Khi thả dây cung
                    drawSound.Stop();
                    Debug.Log("dev16: Shooting arrow.");
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
                Debug.Log("dev16: Spawning new arrow.");

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
                Debug.Log("dev16: Arrow spawned and attached to bowHandle.");
            }
        }

        // Xử lý khi tay phải chạm vào cung
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("RightHand"))
            {
                RightHand = other.transform;
                hasLeftCollider = false; // Reset khi tay vào lại collider
                Debug.Log("dev16: RightHand detected.");
            }
            else
            {
                Debug.Log("dev16: OnTriggerEnter detected non-RightHand object.");
            }
        }

        // Xử lý khi tay phải rời khỏi cung
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("RightHand"))
            {
                hasLeftCollider = true; // Đánh dấu rằng tay đã rời khỏi collider
                Debug.Log("dev16: RightHand exited.");
            }
            else
            {
                Debug.Log("dev16: OnTriggerExit detected non-RightHand object.");
            }
        }

        // Bắn mũi tên
        private void ShootArrow()
        {
            if (currentArrow != null)
            {
                Debug.Log("dev16: Shooting arrow.");

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
                Debug.Log("dev16: Arrow shot with force: " + pullDistance * pullStrengthMultiplier);

                currentArrow = null;
                shootSound.Play();  // Phát âm thanh bắn mũi tên
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
            Debug.Log("dev16: Bow string reset.");
        }
    }
}
