using Fusion;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using multiplayerMode;
using System.Collections;
using System.ComponentModel;
public class BossNetworked : NetworkBehaviour
{
    // Biến mới để theo dõi thời gian không có người chơi tấn công
    private float timeSinceLastPlayerDetected = 0f; // Thời gian đã trôi qua từ lần cuối tìm thấy người chơi
    private float regenTimeThreshold = 15f; // Thời gian cần thiết để hồi HP (15 giây)

    public Transform spawnRubyPos;
    private TickTimer _FakeBodylifeTimer;

  
    [SerializeField]
    private NetworkPrefabRef[] AmethystPrefab;

    // Thêm biến cho hiệu ứng máu
    [SerializeField]
    private ParticleSystem _bloodEffect;
    [SerializeField]
    private Slider healthSlider;

    [SerializeField] private float bossSkillTime = 10f;
    private Animator animator;

    // Hàm này được gọi bởi Unity để vẽ Gizmos
    void OnDrawGizmos()
    {
        // Vẽ hình cầu tại vị trí của GameObject để biểu thị vùng bắt đầu
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius-15);
    }
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
    bool isFakeBoss = false;

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void SetBossFake_RPC()
    {
        isFakeBoss = true;
        if (Object.HasStateAuthority)
        {
            Invoke("DestroyBoss", 10f);
        }

    }
    
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
            healthSlider.value = (float)CurrentHealth / MaxHealth;
        }
        else
        {
            Debug.Log("boss8_5 healthSlider is null");
        }
    }

  
    public void TakeDamage(int damage, Vector3 hitPosition, Vector3 hitNormal, string shooterName, string teamID)
    {
        // Gửi RPC tới máy có State Authority
        RPC_TakeDamage(damage,hitPosition,hitNormal,shooterName,teamID);
    }

    bool isBossDie = false;
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    void RPC_TakeDamage(int damage, Vector3 hitPosition, Vector3 hitNormal, string shooterName, string teamID)
    {
        Debug.Log("boss1takedamage_boss bi ban 111 ");
        if (Object.HasStateAuthority && !isBossDie)
        {
            PlayBloodEffect_RPC(hitPosition, hitNormal);
            //PlayBloodEffect(hitPosition,hitNormal);
            Debug.Log("boss1takedamage_boss bi ban 222");
            CurrentHealth -= damage;
            if (CurrentHealth <= 0)
            {
                if (isFakeBoss)
                {
                    Runner.Despawn(Object);
                    return;
                }
                else
                {
                    CurrentHealth = 0;
                    RPC_Die(shooterName, teamID);
                    isBossDie = true;
                }
               
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
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_Die(string shooterName,string teamID)
    {
      
        NetworkManager.Instance.DestroyBoss_MidFuntion(shooterName,teamID);
        if (Object.HasStateAuthority)
        {
            SpawnRandomAmethysts();
            Invoke("DestroyBoss", 5f);
        }
        DeleteAllChildrenExceptBody(gameObject.transform);
        animator.SetTrigger("Death");
        healthSlider.gameObject.SetActive(false);

       
            
    }
    void DeleteAllChildrenExceptBody(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.name != "Body")
            {
                Destroy(child.gameObject); // Xóa các đối tượng con trừ "Body"
            }
        }
    }

    void DestroyBoss()
    {
        if (Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }
    private void SpawnRandomAmethysts()
    {
        for (int i = 0; i < 10; i++)
        {
            // Chọn ngẫu nhiên một prefab từ AmethystPrefab
            int randomIndex = Random.Range(0, AmethystPrefab.Length);
            NetworkPrefabRef selectedPrefab = AmethystPrefab[randomIndex];

            // Tạo vị trí ngẫu nhiên xung quanh đối tượng hiện tại trong phạm vi x,z = [-5, 5] và y = 0
            Vector3 randomOffset = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-5f, 5f));
            Vector3 spawnPosition = transform.position + randomOffset;

            spawnPosition.y = spawnRubyPos.position.y;

            // Spawn đối tượng tại vị trí ngẫu nhiên với hướng mặc định
            Runner.Spawn(selectedPrefab, spawnPosition, Quaternion.identity);
        }
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
       
        if (Object.HasStateAuthority && !isBossDie)
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
                        // Đặt lại thời gian đếm khi tìm thấy người chơi
                        timeSinceLastPlayerDetected = 0f; // CODE MỚI
                        // Xoay boss về phía người chơi
                        RotateTowards(targetPlayer.position);

                        // Kích hoạt kỹ năng
                        ActivateNextSkill(targetPlayer);

                        // Đặt lại skillTimer
                        skillTimer = TickTimer.CreateFromSeconds(Runner, 5f);
                       
                    }
                    else
                    {
                        // Không tìm thấy người chơi, tăng thời gian đã trôi qua
                        timeSinceLastPlayerDetected += Runner.DeltaTime; // CODE MỚI

                        // Nếu thời gian đã trôi qua vượt quá ngưỡng, hồi đầy HP cho boss
                        if (timeSinceLastPlayerDetected >= regenTimeThreshold) // CODE MỚI
                        {
                            if(Object.HasStateAuthority)
                            {
                                CurrentHealth = MaxHealth; // Hồi đầy HP cho boss
                                //UpdateHealthUI(); // Cập nhật UI
                                
                            }
                            // Reset thời gian đếm
                            timeSinceLastPlayerDetected = 0f; // CODE MỚI

                        }
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
