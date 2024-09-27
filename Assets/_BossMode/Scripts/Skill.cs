using UnityEngine;
using Fusion;

public abstract class Skill : MonoBehaviour
{
    public int SkillId;
    public string SkillName;
    public float CooldownDuration;

    protected BossNetworked boss;
    protected Animator animator;
    protected NetworkRunner runner;

    protected float cooldownTimer = 0f;

    public virtual void Initialize(BossNetworked boss, Animator animator, NetworkRunner runner)
    {
        this.boss = boss;
        this.animator = animator;
        this.runner = runner;
    }

    public bool IsReady => cooldownTimer <= 0f;

    public abstract void Activate();

    protected virtual void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }
}
