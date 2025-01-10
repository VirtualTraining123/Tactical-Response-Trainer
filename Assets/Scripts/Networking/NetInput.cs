using Fusion;
using UnityEngine;

namespace Networking {
  public enum InputButton {
    Shoot, SafetyToggle
  }

  public struct NetInput : INetworkInput {
    public NetworkButtons Buttons;
    public Vector2 Direction;
    public Quaternion LookDirection;
  }
}