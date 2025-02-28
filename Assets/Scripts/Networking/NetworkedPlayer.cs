using System;
using Fusion;
using UnityEngine;

namespace Networking {
  [RequireComponent(typeof(NetworkCharacterController))]
  public class NetworkedPlayer : NetworkBehaviour {
    [Networked] private NetworkCharacterController Controller { get; set; }

    [Header("Player Marker Transforms")] [SerializeField]
    public GameObject leftController;

    [SerializeField] public GameObject rightController;
    [SerializeField] public GameObject gaze;

    [Header("Movement Settings")] [SerializeField]
    private float speed = 5f;

    private Vector3 relativeTransform = Vector3.zero;

    public override void Spawned() {
      base.Spawned();

      Controller = GetComponent<NetworkCharacterController>();
      Debug.Log("Player spawned!!");
    }

    public override void FixedUpdateNetwork() {
      if (!HasStateAuthority) return;
      if (!GetInput<NetInput>(out var input)) return;

      Debug.Log($"Player input: {input.Direction}");

      // Calcular la dirección de movimiento en base a la dirección de la mirada y el input.
      var dir = input.GazeDirection * new Vector3(input.Direction.x, 0, input.Direction.y);
      dir.y = 0;
      relativeTransform += dir.normalized * speed;
      var targetPosition = input.GazePosition + relativeTransform;

      // Mover el PlayerMaker (marca de jugador)
      Controller.Move(speed * dir.normalized * Runner.DeltaTime);

      // Actualizar las posiciones y rotaciones de los elementos del PlayerMarker
      if (gaze != null) {
        gaze.transform.localPosition = input.GazePosition - transform.position * 0.5f;
        gaze.transform.rotation = input.GazeDirection;
      }

      if (leftController != null) {
        leftController.transform.localPosition = input.LeftControllerPosition - transform.position * 0.5f;
        leftController.transform.rotation = input.LeftControllerRotation;
      }

      if (rightController != null) {
        rightController.transform.localPosition = input.RightControllerPosition - transform.position * 0.5f;
        rightController.transform.rotation = input.RightControllerRotation;
      }
    }
  }
}