using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NetworkTransform))]
public class SmallBatEnemy : NetworkBehaviour, IDamageable
{
    public int MaxHealth { get; set; } = 50;

    [Networked(OnChanged = nameof(OnHealthChanged))]
    public int CurrentHealth { get; set; }

  

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private float detectionRadius = 30f; // Bán kính tìm kiếm

    [SerializeField]
    private LayerMask playerLayerMask; // LayerMask cho Layer của người chơi

    [SerializeField]
    private float stopDistance = 1f; // Khoảng cách để dừng lại

    [SerializeField]
    private float moveSpeed = 5f; // Tốc độ di chuyển của quái

    [SerializeField]
    private float lifespan = 15f; // Thời gian sống

    [Networked]
    private TickTimer lifeTimer { get; set; } // Bộ đếm thời gian sống

    private Transform targetPlayer; // Người chơi mục tiêu

    private float targetSearchInterval = 1f; // Khoảng thời gian giữa các lần tìm kiếm mục tiêu
    private float targetSearchTimer = 0f;

    // Biến lưu trữ trạng thái di chuyển hiện tại
    private bool isMoving = false;
    public static void OnHealthChanged(Changed<SmallBatEnemy> changed)
    {
        changed.Behaviour.UpdateHealthUI();
    }
    void UpdateHealthUI()
    {
        Debug.Log("aaa");

    }
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            CurrentHealth = MaxHealth;
            // Gán Animator nếu chưa gán trong Inspector
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }

            lifeTimer = TickTimer.CreateFromSeconds(Runner, lifespan);
            FindAndSetRandomPlayer();
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
        Runner.Despawn(Object);
    }
    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            // Kiểm tra thời gian sống
            if (lifeTimer.Expired(Runner))
            {
                Runner.Despawn(Object);
                return;
            }

            // Cập nhật bộ đếm thời gian tìm kiếm mục tiêu
            targetSearchTimer -= Runner.DeltaTime;
            if (targetSearchTimer <= 0f)
            {
                targetSearchTimer = targetSearchInterval;
                FindAndSetRandomPlayer();
            }

            if (targetPlayer != null)
            {
                float distance = Vector3.Distance(transform.position, targetPlayer.position);

                if (distance > stopDistance)
                {
                    // Di chuyển về phía người chơi
                    MoveTowards(targetPlayer.position);
                }
                else
                {
                    // Dừng lại
                    StopMoving();
                }
            }
            else
            {
                // Không có người chơi mục tiêu
                // Có thể thêm logic khác nếu cần
                StopMoving(); // Dừng lại nếu không có mục tiêu
            }
        }
    }

    void FindAndSetRandomPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayerMask);

        if (hits.Length > 0)
        {
            // Chọn ngẫu nhiên một người chơi từ mảng hits
            int randomIndex = Random.Range(0, hits.Length);
            targetPlayer = hits[randomIndex].transform;
            // Lấy vị trí của người chơi đã chọn
            Vector3 newPos = targetPlayer.position;
            // Hạ trục y của vị trí xuống 0.5f
            newPos.y -= 0.9f;
            // Gán vị trí mới cho người chơi đã chọn
            targetPlayer.position = newPos;
        }
        else
        {
            targetPlayer = null;
        }
    }

    void MoveTowards(Vector3 targetPosition)
    {
        // Nếu chưa ở trạng thái di chuyển, kích hoạt animation "Walk"
        if (!isMoving)
        {
            isMoving = true;
            if (_animator != null)
            {
                _animator.ResetTrigger("Attack"); // Đảm bảo tắt trigger "Attack" nếu đang bật
                _animator.SetTrigger("Walk");
            }
        }

        // Tính toán hướng tới mục tiêu
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Xoay hướng về phía người chơi
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Runner.DeltaTime * 5f);

        // Di chuyển về phía người chơi sử dụng vector hướng
        transform.position += direction * moveSpeed * Runner.DeltaTime;
    }


    void StopMoving()
    {
        // Nếu đang di chuyển, chuyển sang trạng thái dừng và kích hoạt animation "Attack"
        if (isMoving)
        {
            isMoving = false;
            if (_animator != null)
            {
                _animator.ResetTrigger("Walk"); // Đảm bảo tắt trigger "Walk" nếu đang bật
                _animator.SetTrigger("Attack");
            }
        }
        // Thực hiện các hành động khác khi dừng lại, nếu cần
    }
}
