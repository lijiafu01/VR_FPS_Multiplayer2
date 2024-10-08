using Fusion;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
public class BossNetworked : NetworkBehaviour
{
    [SerializeField]
    private Slider healthSlider;

    [SerializeField] private float bossSkillTime = 10f;
    private Animator animator;

    // Danh sách các kỹ năng của boss
    private List<IBossSkill> bossSkills = new List<IBossSkill>();
    [Networked]
    private TickTimer skillTimer { get; set; }

    [Networked]
    private NetworkBool isRotating { get; set; }

    [Networked]
    private NetworkBool hasRotated { get; set; }

    private Vector3 targetRotationDirection;
    [Networked]
    public int MaxHealth { get; set; } = 100;

    [Networked(OnChanged = nameof(OnHealthChanged))]
    public int CurrentHealth { get; set; }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            // Khởi tạo bộ đếm thời gian kỹ năng
            skillTimer = TickTimer.CreateFromSeconds(Runner, bossSkillTime);
            isRotating = false;
            hasRotated = false;

            // Khởi tạo HP của boss
            CurrentHealth = MaxHealth;
        }

        // (Các mã khác trong hàm Spawned)
    }
    public static void OnHealthChanged(Changed<BossNetworked> changed)
    {
        changed.Behaviour.UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            Debug.Log("boss8_5 cogoi");
            healthSlider.value = (float)CurrentHealth / MaxHealth;
        }
        else
        {
            Debug.Log("boss8_5 healthSlider is null");
        }
    }



    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)
        {
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Die();
            }
        }
    }

    void Die()
    {
        CurrentHealth = MaxHealth;
    }
    void Awake()
    {
        Debug.Log("boss7_1");
        animator = GetComponent<Animator>();

        // Lấy các kỹ năng từ các component trên các đối tượng con
        var skillComponents = GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var comp in skillComponents)
        {
            // Bỏ qua các component trên chính đối tượng boss
            if (comp.gameObject == this.gameObject)
                continue;

            if (comp is IBossSkill skill)
            {
                bossSkills.Add(skill);
                skill.OnSkillStart += OnSkillStart;
                skill.OnSkillEnd += OnSkillEnd;
                Debug.Log("boss7_3");
            }
        }
        Debug.Log("boss7_2");

    }


    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            // Cập nhật các kỹ năng
            foreach (var skill in bossSkills)
            {
                skill.FixedUpdateSkill();
            }

            // Kiểm tra nếu boss đang casting bất kỳ kỹ năng nào
            bool isCasting = bossSkills.Exists(skill => skill.IsCasting);
            if (isRotating)
            {
                RotateTowards(targetRotationDirection);
            }
            else if (!isCasting)
            {
                // Kiểm tra nếu skillTimer đã hết hạn
                if (skillTimer.Expired(Runner))
                {

                    // Tìm người chơi gần nhất
                    Transform targetPlayer = FindNearestPlayer();

                    if (targetPlayer != null)
                    {

                        // Xoay boss về phía người chơi
                        RotateTowards(targetPlayer.position);

                        // Kích hoạt kỹ năng
                        ActivateNextSkill(targetPlayer);

                        // Đặt lại skillTimer
                        skillTimer = TickTimer.CreateFromSeconds(Runner, 5f);
                       
                    }
                }
            }
            else
            {
                // Bắt đầu xoay về phía người chơi
                Transform targetPlayer = FindNearestPlayer();

                if (targetPlayer != null)
                {
                    // Lưu hướng mục tiêu
                    targetRotationDirection = targetPlayer.position;

                    // Bắt đầu xoay
                    isRotating = true;
                }
            }
        }
    }
    void ActivateNextSkill(Transform targetPlayer)
    {
        // Tạo danh sách các kỹ năng sẵn sàng
        var availableSkills = bossSkills.FindAll(skill => !skill.IsOnCooldown);

        if (availableSkills.Count > 0)
        {
            // Chọn ngẫu nhiên một kỹ năng
            int randomIndex = Random.Range(0, availableSkills.Count);
            var skill = availableSkills[randomIndex];

            skill.ActivateSkill(targetPlayer);
        }
        else
        {
            // Không có kỹ năng nào sẵn sàng, boss chờ đến lần sau
        }
    }

    void OnSkillStart()
    {
        // Xử lý khi kỹ năng bắt đầu (nếu cần)
        // Ví dụ: Dừng di chuyển, kích hoạt animation, v.v.
    }

    void OnSkillEnd()
    {
        // Xử lý khi kỹ năng kết thúc (nếu cần)
        // Ví dụ: Tiếp tục di chuyển, v.v.
    }

    [SerializeField]
    private float detectionRadius = 30f; // Bán kính tìm kiếm

    [SerializeField]
    private LayerMask playerLayerMask; // LayerMask cho Layer của người chơi

    Transform FindNearestPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayerMask);

        Transform nearestPlayer = null;
        float minDistance = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float distance = Vector3.Distance(transform.position, hit.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPlayer = hit.transform;
            }
        }

        return nearestPlayer;
    }


    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0f; // Chỉ xoay trên trục Y

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float rotationSpeed = 5f; // Tốc độ xoay

            // Xoay từ từ về phía mục tiêu
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime * 100f);

            // Kiểm tra nếu góc giữa hướng hiện tại và hướng mục tiêu nhỏ hơn một ngưỡng
            float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
            if (angleDifference < 1f)
            {
                // Đã xoay xong
                isRotating = false;
                hasRotated = true;
            }
        }
    }
}