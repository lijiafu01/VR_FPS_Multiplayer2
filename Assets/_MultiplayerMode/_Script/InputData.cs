using Fusion;
using UnityEngine;
using multiplayerMode;
namespace multiplayerMode
{
public enum InputButton
{
    Jump,
    Fire,
    Fire2,
    Bow
}

public struct InputData : INetworkInput
{
    public NetworkButtons Button;
    public Vector2 MoveInput;
    public Angle Pitch;
    public Angle Yaw;
}
}
