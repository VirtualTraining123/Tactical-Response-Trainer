using Fusion;
using UnityEngine;

namespace Networking {
  public enum InputButton {
    Shoot, SafetyToggle
  }

  public struct NetInput : INetworkInput {
    public NetworkButtons Buttons;
    public Vector2 Direction;
    
    public Vector3 GazePosition;
    public Quaternion GazeDirection; // Note: Maybe someone can cheat with this.

    public Vector3 LeftControllerPosition;
    public Quaternion LeftControllerRotation;
    
    public Vector3 RightControllerPosition;
    public Quaternion RightControllerRotation;
  }
}