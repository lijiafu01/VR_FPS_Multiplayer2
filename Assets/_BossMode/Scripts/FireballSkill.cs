/*using UnityEngine;
using Fusion;

public class FireballSkill : Skill
{
    public NetworkPrefabRef fireballPrefab; // Tham chiếu đến prefab quả cầu lửa
    public Transform firePoint;
    public float searchRadius = 20f; // Phạm vi tìm kiếm người chơi
    public float rotationSpeed = 90f; // Tốc độ xoay của boss
    public float launchAngle = 45f; // Góc phóng

    private bool isSkillActive = false;
    private GameObject targetPlayer;
    private Vector3 targetPosition;
    private SkillState currentState = SkillState.None;

    private TickTimer fireballDelayTimer; // Bộ đếm thời gian cho việc sinh ra quả cầu lửa

    private enum SkillState
    {
        None,
        FindingTarget,
        Rotating,
        WaitingToFire,
        Cooldown
    }

    public override void Activate()
    {
        isSkillActive = true;
        if (runner.IsSharedModeMasterClient && IsReady)
        {
           
            currentState = SkillState.FindingTarget;

            // Kích hoạt animation tấn công
            animator.SetInteger("State", 2); // 2 tương ứng với trạng thái Attacking
        }
    }
   
    protected override void Update()
    {
        base.Update();
        if (runner == null)
        {
            return;
        }
        if (!runner.IsSharedModeMasterClient)
            return;

        if (isSkillActive)
        {
            switch (currentState)
            {
                case SkillState.FindingTarget:
                    FindNearestPlayer();
                    break;
                case SkillState.Rotating:
                    RotateTowardsTarget();
                    break;
                case SkillState.WaitingToFire:
                    WaitAndFire();
                    break;
                case SkillState.Cooldown:
                    CheckAnimationFinished();
                    break;
            }
        }
    }

    private void FindNearestPlayer()
    {
        targetPlayer = FindNearestPlayerInRange();
        if (targetPlayer != null)
        {
            targetPosition = targetPlayer.transform.position;
            currentState = SkillState.Rotating;
        }
        else
        {
            // Không tìm thấy người chơi, kết thúc kỹ năng
            EndSkill();
        }
    }

    private GameObject FindNearestPlayerInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(boss.transform.position, searchRadius);
        GameObject nearestPlayer = null;
        float minDistance = Mathf.Infinity;

        foreach (var collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                float distance = Vector3.Distance(boss.transform.position, collider.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPlayer = collider.gameObject;
                }
            }
        }
        return nearestPlayer;
    }

    private void RotateTowardsTarget()
    {
        // Kiểm tra xem mục tiêu có còn trong phạm vi không
        if (Vector3.Distance(boss.transform.position, targetPosition) > searchRadius)
        {
            EndSkill(); // Hủy kỹ năng nếu mục tiêu rời khỏi phạm vi
            return;
        }

        Vector3 direction = (targetPosition - boss.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        boss.transform.rotation = Quaternion.RotateTowards(boss.transform.rotation, targetRotation, rotationSpeed * runner.DeltaTime);

        float angleDifference = Quaternion.Angle(boss.transform.rotation, targetRotation);
        if (angleDifference < 1f)
        {
            // Xoay hoàn tất, chờ 0.3 giây rồi bắn
            fireballDelayTimer = TickTimer.CreateFromSeconds(runner, 0.2f);
            currentState = SkillState.WaitingToFire;
        }
    }

    private void WaitAndFire()
    {
        // Kiểm tra xem mục tiêu có còn trong phạm vi không
        if (Vector3.Distance(boss.transform.position, targetPosition) > searchRadius)
        {
            EndSkill(); // Hủy kỹ năng nếu mục tiêu rời khỏi phạm vi
            return;
        }

        if (fireballDelayTimer.Expired(runner))
        {
            FireAtTarget();
        }
    }


    private void FireAtTarget()
    {
        Vector3 launchVelocity = CalculateLaunchVelocity(firePoint.position, targetPosition, launchAngle);

        if (launchVelocity == Vector3.zero)
        {
            // Mục tiêu không thể đạt được
            EndSkill();
            return;
        }

        // Sinh ra quả cầu lửa và truyền launchVelocity
        runner.Spawn(fireballPrefab, firePoint.position, Quaternion.identity, null, (runner, obj) =>
        {
            var fireball = obj.GetComponent<Fireball>();
            fireball.networkedLaunchVelocity = launchVelocity;
        });

        // Đặt cooldown cho kỹ năng
        cooldownTimer = CooldownDuration;
        currentState = SkillState.Cooldown;
    }

    private void CheckAnimationFinished()
    {
        // Kiểm tra nếu animation đã chạy xong
        AnimatorStateInfo currentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (currentStateInfo.IsName("Attack") && currentStateInfo.normalizedTime >= 1f && !animator.IsInTransition(0))
        {
            animator.SetInteger("State", 0); // Quay lại trạng thái Idle
            EndSkill();
        }
    }

    private void EndSkill()
    {
        isSkillActive = false;
        currentState = SkillState.None;

        // Đặt trạng thái boss về idle hoặc hành động tiếp theo
        animator.SetInteger("State", 0); // Quay lại trạng thái Idle

        // Thông báo cho Boss rằng kỹ năng đã kết thúc
        boss.OnSkillCompleted();
    }


    private Vector3 CalculateLaunchVelocity(Vector3 startPoint, Vector3 targetPoint, float angle)
    {
        float gravity = Physics.gravity.magnitude;
        float radianAngle = angle * Mathf.Deg2Rad;

        Vector3 planarTarget = new Vector3(targetPoint.x, 0, targetPoint.z);
        Vector3 planarPosition = new Vector3(startPoint.x, 0, startPoint.z);

        float distance = Vector3.Distance(planarTarget, planarPosition);
        float yOffset = startPoint.y - targetPoint.y;

        float initialVelocity = (1 / Mathf.Cos(radianAngle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(radianAngle) + yOffset));

        if (float.IsNaN(initialVelocity))
        {
            // Mục tiêu không thể đạt được
            return Vector3.zero;
        }

        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(radianAngle), initialVelocity * Mathf.Cos(radianAngle));
        Vector3 direction = (planarTarget - planarPosition).normalized;

        Vector3 launchVelocity = direction * velocity.z;
        launchVelocity.y = velocity.y;

        return launchVelocity;
    }
}
*/