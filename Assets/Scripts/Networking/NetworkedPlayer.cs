using System;
using Fusion;
using UnityEngine;

namespace Networking {
  [RequireComponent(typeof(NetworkCharacterController))]
  public class NetworkedPlayer : NetworkBehaviour {
    [Networked] private NetworkCharacterController Controller { get; set; }
    [SerializeField] private float speed = 5f;
    [SerializeField] private Vector3 relativeTransform = Vector3.zero;


    private void Start() {
      Controller = gameObject.GetComponent<NetworkCharacterController>();
    }

    public override void Spawned() {
      base.Spawned();
      Debug.Log("Player spawned!!");
    }

    public override void FixedUpdateNetwork() {
      if (!HasStateAuthority) return;
      if (!GetInput<NetInput>(out var input)) return;
      Debug.Log($"Player input: {input.Direction}");
      // Move the player based on the input
      // Add to position
      var dir = input.GazeDirection * new Vector3(input.Direction.x, 0, input.Direction.y);
      dir.y = 0;
      relativeTransform += dir.normalized * speed;
      var targetPosition = input.GazePosition + relativeTransform;
      // transform.position = targetPosition;
      // // Shoot if the shoot button is pressed
      // if (input.Buttons.IsSet(InputButton.Shoot)) {
      //   Shoot();
      // }
      
      Controller.Move(speed * dir.normalized * Runner.DeltaTime);
    }

    private void Shoot() {
      throw new System.NotImplementedException();
    }
  }
}