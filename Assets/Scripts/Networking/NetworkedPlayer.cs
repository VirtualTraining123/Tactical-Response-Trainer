using System;
using System.Collections.Generic;
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
    [SerializeField] public List<GameObject> playerHead;

    [Header("Movement Settings")] [SerializeField]
    private float speed = 5f;

    private Vector3 relativeTransform = Vector3.zero;

    public override void Spawned() {
      base.Spawned();

      Controller = GetComponent<NetworkCharacterController>();
      Debug.Log("Player spawned!!");
      if (HasStateAuthority) {
        playerHead.ForEach(x =>
        {
          if (x.TryGetComponent<MeshRenderer>(out var meshRenderer)) meshRenderer.enabled = false;
          if (x.TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer)) skinnedMeshRenderer.enabled = false;
        });
      }
    }

    public override void FixedUpdateNetwork() {
      if (!HasStateAuthority) return;
      if (!GetInput<NetInput>(out var input)) return;

      // Debug.Log($"Player input: {input.Direction}");

      // Calcular la dirección de movimiento en base a la dirección de la mirada y el input.
      var dir = input.GazeDirection * new Vector3(input.Direction.x, 0, input.Direction.y);
      dir.y = 0;
      relativeTransform += dir.normalized * speed;
      // var targetPosition = input.GazePosition + relativeTransform;

      Debug.Log($"Player input: {speed * dir.normalized * Runner.DeltaTime}; Velocity ${Controller.Velocity.magnitude}");
      // Mover el PlayerMaker (marca de jugador)
      if (Controller.Velocity.magnitude > 10)
      {
        Controller.Velocity = Vector3.zero;
      }
      Controller.Move(speed * dir.normalized * Runner.DeltaTime);

      // Actualizar las posiciones y rotaciones de los elementos del PlayerMarker
      if (gaze != null) {
        gaze.transform.position = input.GazePosition + transform.position;
        gaze.transform.rotation = input.GazeDirection;
      }

      if (leftController != null) {
        leftController.transform.position = input.LeftControllerPosition + transform.position;
        leftController.transform.rotation = input.LeftControllerRotation;
      }

      if (rightController != null) {
        rightController.transform.position = input.RightControllerPosition + transform.position;
        rightController.transform.rotation = input.RightControllerRotation;
      }
    }
  }
}