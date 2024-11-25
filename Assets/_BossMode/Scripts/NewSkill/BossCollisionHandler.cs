using UnityEngine;
using Fusion;
public class BossCollisionHandler : NetworkBehaviour
{
    private PowerfulJumpSkill jumpSkill;
    void Awake()
    {
        // Lấy tham chiếu đến PowerfulJumpSkill trên đối tượng con
        jumpSkill = GetComponentInChildren<PowerfulJumpSkill>();
    }
    void OnCollisionEnter(Collision collision)
    {
        if ((Runner == null))
        {
            return;
        }
        // Chuyển tiếp sự kiện va chạm đến PowerfulJumpSkill
        jumpSkill.HandleCollision(collision);
    }
}
