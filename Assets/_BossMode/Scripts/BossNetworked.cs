using Fusion;
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
                Debug.Log("boss1_112");
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
}
