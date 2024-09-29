using Fusion;
using UnityEngine;

public class BombEF : NetworkBehaviour
{
    private TickTimer lifeTimer;
    public override void Spawned()
    {
        Debug.Log("boss2_bomb ef2");
        // Khởi tạo TickTimer để hủy đối tượng sau 2 giây
        lifeTimer = TickTimer.CreateFromSeconds(Runner, 2f);
    }
    public override void FixedUpdateNetwork()
    {
        // Kiểm tra nếu TickTimer đã hết hạn
        if (lifeTimer.Expired(Runner))
        {
            Debug.Log("boss2_bomb ef3");
            // Hủy đối tượng trên mạng
            Runner.Despawn(Object);
        }
    }
}
