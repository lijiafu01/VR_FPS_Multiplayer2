/*using UnityEngine;
using Fusion;

public class SkillManager : MonoBehaviour
{
    public Skill[] _skills;
    public int SkillCount => _skills.Length;

    private NetworkRunner runner;
    private BossNetworked boss;
    private Animator animator;

    private void Awake()
    {
        _skills = GetComponentsInChildren<Skill>();

        // Lấy tham chiếu đến BossNetworked và Animator
        boss = GetComponentInParent<BossNetworked>();
        animator = GetComponentInParent<Animator>();
    }

    public void Initialize(NetworkRunner runner)
    {
        this.runner = runner;

        // Khởi tạo từng kỹ năng
        foreach (var skill in _skills)
        {
            skill.Initialize(boss, animator, runner);
        }
    }

    public void ActivateSkill(int skillId)
    {
        foreach (var skill in _skills)
        {
            if (skill.SkillId == skillId && skill.IsReady)
            {
                skill.Activate();
                break;
            }
        }
    }
}
*/