using Fusion;
using JetBrains.Annotations;
using UnityEngine;

namespace Networking {
  public class NetworkedPlayer: NetworkBehaviour {
    [SerializeField] private float speed = 5f;
    [SerializeField] private Vector3 relativeTransform = Vector3.zero;

    public override void Spawned() {
      base.Spawned();
      Debug.Log("Player spawned!!");
    }

    public override void FixedUpdateNetwork() {
      if (!GetInput<NetInput>(out var input)) return;
      Debug.Log($"Player input: {input.Direction}");
      // Move the player based on the input
      // Add to position
      var dir = input.GazeDirection * new Vector3(input.Direction.x, 0, input.Direction.y);
      dir.y = 0;
      relativeTransform += dir.normalized * speed;
      transform.position = input.GazePosition + relativeTransform;
      // Shoot if the shoot button is pressed
      if (input.Buttons.IsSet(InputButton.Shoot)) {
        Shoot();
      }
    }
    
    private void Shoot() {
      throw new System.NotImplementedException();
    }
  }
}
