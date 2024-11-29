using UnityEngine;
using Fusion;
namespace multiplayerMode
{
    public class Mount : NetworkBehaviour
    {
        [SerializeField] private float flySpeed = 5f;  // Tốc độ bay của thú cưỡi
        [SerializeField] private Transform[] flightPath;  // Mảng vị trí (Transform) mà thú cưỡi sẽ bay qua
        private int currentTargetIndex = 0;  // Chỉ mục của vị trí hiện tại
        private bool isFlying = false;  // Trạng thái bay của thú cưỡi
        private GameObject currentPlayer = null;  // Người chơi hiện tại đang cưỡi
        private Rigidbody rb;
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody is missing from Mount.");
            }
        }
        void Update()
        {
            if (isFlying && currentPlayer != null)
            {
                MoveAlongPath();  // Thú cưỡi sẽ bay theo các vị trí khi người chơi cưỡi
            }
        }
        // Phương thức để thú cưỡi bay theo các vị trí đã đặt
        private void MoveAlongPath()
        {
            if (currentTargetIndex >= flightPath.Length) return;
            Transform targetPosition = flightPath[currentTargetIndex];
            float step = flySpeed * Time.deltaTime;
            // Di chuyển thú cưỡi đến vị trí tiếp theo
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, step);
            // Nếu thú cưỡi đã đến vị trí mục tiêu, chuyển sang vị trí tiếp theo
            if (Vector3.Distance(transform.position, targetPosition.position) < 0.1f)
            {
                currentTargetIndex++;
                if (currentTargetIndex >= flightPath.Length)
                {
                    currentTargetIndex = 0;  // Lặp lại đường bay nếu đến cuối
                }
            }
        }
        // Bắt đầu cho thú cưỡi bay theo người chơi
        public void StartFlying(GameObject player)
        {
            isFlying = true;
            currentPlayer = player;
        }
        // Dừng thú cưỡi khi người chơi không còn cưỡi
        public void StopFlying()
        {
            isFlying = false;
            currentPlayer = null;
        }
        // Khi thú cưỡi va chạm với người chơi, bắt đầu bay
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !isFlying)
            {
                StartFlying(other.gameObject);
            }
        }
        // Khi người chơi không còn va chạm với thú cưỡi, dừng bay
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && isFlying)
            {
                StopFlying();
            }
        }
    }
}
