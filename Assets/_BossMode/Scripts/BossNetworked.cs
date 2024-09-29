using Fusion;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BossNetworked : NetworkBehaviour
{
    // Biến mạng lưu trữ chiêu thức hiện tại
    [Networked(OnChanged = nameof(OnSkillChanged))]
    public int CurrentSkillId { get; set; }

    private SkillManager _skillManager;
    private Animator _animator;
    private bool wasSharedModeMaster = false;

    private void Awake()
    {
        _skillManager = GetComponent<SkillManager>();
        _animator = GetComponent<Animator>();
    }

    public override void Spawned()
    {
        // Khởi tạo SkillManager với NetworkRunner
        _skillManager.Initialize(Runner);

        // Kiểm tra nếu máy khách này là Shared Mode Master
        if (Runner.IsSharedModeMasterClient)
        {
            CurrentSkillId = -1; // Không có chiêu thức nào đang kích hoạt
        }
        wasSharedModeMaster = Runner.IsSharedModeMasterClient;
    }

    public override void FixedUpdateNetwork()
    {
        // Kiểm tra nếu Runner.IsSharedModeMasterClient đã thay đổi
        if (wasSharedModeMaster != Runner.IsSharedModeMasterClient)
        {
            wasSharedModeMaster = Runner.IsSharedModeMasterClient;
            if (Runner.IsSharedModeMasterClient)
            {
                Debug.Log("[BossNetworked] This client is now the Shared Mode Master.");

                // Thực hiện khởi tạo hoặc cập nhật cần thiết
                if (CurrentSkillId == -1)
                {
                    // Không có kỹ năng nào đang hoạt động, có thể bắt đầu logic
                    HandleBossLogic();
                }
            }
        }
        if (Runner.IsSharedModeMasterClient)
        {
            HandleBossLogic();
        }
    }
    private void HandleBossLogic()
    {
        Debug.Log("boss1_111");
        // Kiểm tra nếu Boss đang không thực hiện kỹ năng
        if (CurrentSkillId == -1)
        {
            // Kiểm tra xem có nên sử dụng kỹ năng không
            if (ShouldUseSkill())
            {
                int skillId = ChooseSkill();
                CurrentSkillId = skillId;
                Debug.Log($"[Boss] Chose Skill ID: {skillId}");
            }
        }
    }
    private bool ShouldUseSkill()
    {
        // Điều kiện để Boss sử dụng kỹ năng
        return Random.value < 0.01f; // Ví dụ: 1% cơ hội mỗi frame
    }
    private int ChooseSkill()
    {
        var readySkills = _skillManager._skills.Where(skill => skill.IsReady).ToList();
        if (readySkills.Count > 0)
        {
            int index = Random.Range(0, readySkills.Count);
            return readySkills[index].SkillId;
        }
        else
        {
            // Không có chiêu thức nào sẵn sàng
            return -1;
        }
    }
    // Callback khi CurrentSkillId thay đổi
    private static void OnSkillChanged(Changed<BossNetworked> changed)
    {
        changed.Behaviour.OnSkillChanged();
    }

    private void OnSkillChanged()
    {
        if (CurrentSkillId >= 0)
        {
            // Kích hoạt kỹ năng tương ứng
            _skillManager.ActivateSkill(CurrentSkillId);
        }
    }
    // Hàm này được gọi bởi Skill khi kết thúc
    public void OnSkillCompleted()
    {
        if (Runner.IsSharedModeMasterClient)
        {
            CurrentSkillId = -1; // Reset để có thể chọn kỹ năng mới
        }
    }
    //---
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcStartJump(Vector3 targetPosition, float jumpDuration)
    {
        StartCoroutine(HandleJump(targetPosition, jumpDuration));
    }

    private IEnumerator HandleJump(Vector3 targetPosition, float jumpDuration)
    {
        // Kích hoạt animation nhảy
        _animator.SetInteger("State", 3); // 3 tương ứng với trạng thái Jumping

        // Kích hoạt trọng lực và đặt isKinematic = false
        Rigidbody bossRigidbody = GetComponent<Rigidbody>();
        bossRigidbody.useGravity = true;
        // Tính toán vận tốc nhảy
        Vector3 jumpVelocity = CalculateJumpVelocity(transform.position, targetPosition, jumpDuration);
        bossRigidbody.velocity = Vector3.zero;
        bossRigidbody.AddForce(jumpVelocity, ForceMode.VelocityChange);

        // Chờ đợi trong thời gian nhảy
        yield return new WaitForSeconds(jumpDuration);

        // Kết thúc nhảy
        bossRigidbody.useGravity = false;
        bossRigidbody.velocity = Vector3.zero;

        // Chuyển về trạng thái Idle
        _animator.SetInteger("State", 0);

        // Thông báo kết thúc kỹ năng
        OnSkillCompleted();
    }

    private Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 targetPoint, float timeToTarget)
    {
        // Tính toán sự chênh lệch vị trí
        Vector3 distance = targetPoint - startPoint;

        // Tính toán vận tốc cần thiết
        Vector3 velocityY = Vector3.up * distance.y / timeToTarget - Vector3.up * 0.5f * Physics.gravity.y * timeToTarget;
        Vector3 velocityXZ = new Vector3(distance.x, 0, distance.z) / timeToTarget;

        // Kết hợp vận tốc trục Y và XZ
        return velocityXZ + velocityY;
    }
}
