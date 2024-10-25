using Fusion;
using multiplayerMode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(NetworkTransform))]
public class SmallBatEnemy : NetworkBehaviour, IDamageable
{
    public Transform ArrowPos;
    // Thêm biến cho hiệu ứng máu
    [SerializeField]
    private ParticleSystem _bloodEffect;
    [SerializeField]
    private Slider healthSlider;
    // Biến đếm thời gian giữa các lần gây sát thương
    private float damageCooldownTimer = 0f;

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

    [SerializeField]
    private float heightOffset = -1.0f; // Độ lệch theo trục Y so với mục tiêu (âm nghĩa là thấp hơn)

    public static void OnHealthChanged(Changed<SmallBatEnemy> changed)
    {
        changed.Behaviour.UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)CurrentHealth / MaxHealth;
        }
        else
        {
            Debug.Log("boss8_5 healthSlider is null");
        }
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
    public void TakeDamage(int damage, Vector3 hitPosition, Vector3 hitNormal, string shooterName,string teamID)
    {
        // Gửi RPC tới máy có State Authority
        RPC_TakeDamage(damage,hitPosition,hitNormal,shooterName,teamID);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_TakeDamage(int damage, Vector3 hitPosition, Vector3 hitNormal, string shooterName, string teamID)
    {
        Debug.Log("boss1takedamage_boss bi ban 111");

        if (Object.HasStateAuthority)
        {
            PlayBloodEffect_RPC(hitPosition, hitNormal);
            Debug.Log("boss1takedamage_boss bi ban 222");
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                CurrentHealth = 0;
                Die();
            }
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void PlayBloodEffect_RPC(Vector3 hitPosition, Vector3 hitNormal)
    {

        if (_bloodEffect != null)
        {
            // Đặt vị trí và hướng của ParticleSystem dựa trên vị trí va chạm và pháp tuyến
            _bloodEffect.transform.position = hitPosition;
            _bloodEffect.transform.rotation = Quaternion.LookRotation(hitNormal);

            _bloodEffect.Play();
            Invoke(nameof(StopBloodEffect), 1f); // Dừng hiệu ứng sau 1 giây
        }
        else
        {
            Debug.Log("dev_blood_effect_is_null");
        }
    }
    private void StopBloodEffect()
    {
        if (_bloodEffect != null)
        {
            _bloodEffect.Stop();
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
            // Cập nhật bộ đếm thời gian gây sát thương
            if (damageCooldownTimer > 0f)
            {
                damageCooldownTimer -= Runner.DeltaTime;
            }
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

        // Tính toán hướng tới mục tiêu, bỏ qua trục Y
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Loại bỏ thành phần Y
        direction = direction.normalized;

        // Xoay hướng về phía người chơi (chỉ xoay quanh trục Y)
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Runner.DeltaTime * 5f);
        }

        // Điều chỉnh vị trí Y của quái nhỏ
        Vector3 newPosition = transform.position + direction * moveSpeed * Runner.DeltaTime;
        newPosition.y = targetPlayer.position.y + heightOffset; // Điều chỉnh độ cao so với mục tiêu

        // Cập nhật vị trí của quái nhỏ
        transform.position = newPosition;
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

        // Giữ nguyên vị trí Y của quái nhỏ
        if (targetPlayer != null)
        {
            Vector3 position = transform.position;
            position.y = targetPlayer.position.y + heightOffset;
            transform.position = position;
        }

        // Thực hiện các hành động khác khi dừng lại, nếu cần
    }
    private void OnTriggerStay(Collider other)
    {
        if (Runner == null) return;
        if (!Object.HasStateAuthority)
            return;

        // Chỉ kiểm tra va chạm khi quái vật đã dừng lại
        if (!isMoving)
        {
            // Kiểm tra xem đối tượng va chạm có phải là người chơi không
            if (other.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
            {
                // Chỉ gây sát thương khi damageCooldownTimer <= 0
                if (damageCooldownTimer <= 0f)
                {
                    Debug.Log("Bossfixbug_ " + player.playerName);
                    // Gây sát thương cho người chơi
                    ApplyDamageToPlayer(player);
                    RPC_TriggerAttackAnimation();

                    // Reset bộ đếm thời gian về 2 giây
                    damageCooldownTimer = 2f;
                }
            }
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_TriggerAttackAnimation()
    {

        if (_animator != null)
        {
            _animator.ResetTrigger("Walk");
            _animator.SetTrigger("Attack");
        }
    }
    public int Damage = 1;
    void ApplyDamageToPlayer(PlayerController player)
    {
        if (player != null)
        {
            Debug.Log("Bossfixbug_ 2_" + player.playerName);

            player.RPC_TakeDamage(Damage);
        }
    }
}
