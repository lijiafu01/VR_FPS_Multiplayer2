using UnityEngine;


public class FireballSkill : Skill
{
    public GameObject fireballPrefab;
    public Transform firePoint;

    private bool isSkillActive = false;

    public override void Activate()
    {
        if (runner.IsSharedModeMasterClient && IsReady)
        {
            isSkillActive = true;
            //cooldownTimer = TickTimer.CreateFromSeconds(runner, CooldownDuration);

            // Kích hoạt animation
            animator.SetInteger("State", 2); // 2 tương ứng với trạng thái Attacking
            
        }
        Invoke("FireBall", 0.3f);
    }
    private void FireBall()
    {
        runner.Spawn(fireballPrefab, firePoint.position, firePoint.rotation);

    }
    protected override void Update()
    {
        base.Update();

        if (isSkillActive)
        {
            // Lấy thông tin về trạng thái hiện tại của Animator trong Layer 0
            AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // Kiểm tra nếu Animator đang ở trạng thái "Attack"
            if (currentStateInfo.IsName("Attack"))
            {
                // Kiểm tra nếu animation đã chạy xong
                if (currentStateInfo.normalizedTime >= 1f && !animator.IsInTransition(0))
                {
                    isSkillActive = false;

                    animator.SetInteger("State", 0); // Quay lại trạng thái Idle

                    // Thông báo cho Boss rằng kỹ năng đã kết thúc
                    if (runner.IsSharedModeMasterClient)
                    {
                        boss.OnSkillCompleted();
                    }
                }
            }
        }
    }
}
