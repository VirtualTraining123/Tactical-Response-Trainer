using Fusion;
using UnityEngine;

namespace Networking {
  public class NetworkedPlayer: NetworkBehaviour {
    [SerializeField] private float speed = 5f;

    public override void Spawned() {
      base.Spawned();
      Debug.Log("Player spawned!!");
    }

    public override void FixedUpdateNetwork() {
      if (!GetInput<NetInput>(out var input)) return;
      Debug.Log($"Player input: {input.Direction}");
      // Move the player based on the input
      // Add to position
      var dir = input.LookDirection * new Vector3(input.Direction.x, 0, input.Direction.y);
      transform.position += dir.normalized * speed;
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
