using Fusion;
public class BombEF : NetworkBehaviour
{
    private TickTimer lifeTimer;
    public override void Spawned()
    {
        lifeTimer = TickTimer.CreateFromSeconds(Runner, 2f);
    }
    public override void FixedUpdateNetwork()
    {
        if (lifeTimer.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }
}
