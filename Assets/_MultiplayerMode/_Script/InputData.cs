using Fusion;
using UnityEngine;

public enum InputButton
{
    Jump,
    Fire,
    Fire2
}

public struct InputData : INetworkInput
{
    public NetworkButtons Button;
    public Vector2 MoveInput;
    public Angle Pitch;
    public Angle Yaw;
}