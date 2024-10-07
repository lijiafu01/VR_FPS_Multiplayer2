/*using UnityEngine;

public class StrongJumpSkill : Skill
{
    public float jumpDuration = 2f; // Thời gian để hoàn thành cú nhảy
    public float searchRadius = 20f; // Phạm vi tìm kiếm người chơi
    public float jumpProximityFactor = 0.75f; // Khoảng cách tới người chơi khi nhảy (tỷ lệ phần trăm)

    private bool isSkillActive = false;
    private GameObject targetPlayer;
    private Vector3 targetPosition;

    public override void Activate()
    {
        if (IsReady)
        {
            isSkillActive = true;

            // Tìm người chơi gần nhất
            targetPlayer = FindNearestPlayerInRange();
            if (targetPlayer != null)
            {
                Vector3 bossPosition = boss.transform.position;
                Vector3 playerPosition = targetPlayer.transform.position;
                // Tính toán vị trí nhảy dựa trên tỷ lệ khoảng cách
                targetPosition = bossPosition + jumpProximityFactor * (playerPosition - bossPosition);

                // Chỉ gọi RPC từ State Authority
                if (boss.Object.HasStateAuthority)
                {
                    boss.RpcStartJump(targetPosition, jumpDuration);
                }
            }
            else
            {
                // Không tìm thấy người chơi, kết thúc kỹ năng
                EndSkill();
            }
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

    private void EndSkill()
    {
        isSkillActive = false;
        // Đặt cooldown cho kỹ năng
        cooldownTimer = CooldownDuration;
        // Thông báo cho Boss rằng kỹ năng đã kết thúc
        boss.OnSkillCompleted();
    }
}
*/