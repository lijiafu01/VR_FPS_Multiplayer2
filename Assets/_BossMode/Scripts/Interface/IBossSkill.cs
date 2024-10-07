using UnityEngine;

public interface IBossSkill
{
    // Tên của kỹ năng
    string SkillName { get; }

    // Thời gian hồi chiêu
    float Cooldown { get; }

    // Thời gian casting
    float CastingDuration { get; }

    // Kiểm tra xem kỹ năng có đang hồi chiêu không
    bool IsOnCooldown { get; }

    // Kiểm tra xem kỹ năng có đang được sử dụng không
    bool IsCasting { get; }

    // Kích hoạt kỹ năng
    void ActivateSkill(Transform target);

    // Cập nhật trạng thái của kỹ năng
    void FixedUpdateSkill();

    // Sự kiện khi kỹ năng bắt đầu
    event System.Action OnSkillStart;

    // Sự kiện khi kỹ năng kết thúc
    event System.Action OnSkillEnd;
}
