using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Projectiles;

namespace Networking {
  [RequireComponent(typeof(NetworkCharacterController))]
  public class NetworkedPlayer : NetworkBehaviour {
    [Networked] private NetworkCharacterController Controller { get; set; }

    [Header("Player Marker Transforms")]
    [SerializeField]
    public GameObject leftController;

    [SerializeField] public GameObject rightController;
    [SerializeField] public GameObject gaze;
    [SerializeField] public List<GameObject> playerHead;
    public double distanceBeforeCorrection = 0.25;

    [Header("Movement Settings")]
    [SerializeField]
    private float speed = 5f;

    private Vector3 relativeTransform = Vector3.zero;
    // [HideInInspector]
    [Networked]
    public Weapon_ProjectileDataBuffer_Hitscan CurrentWeapon { get; set; }

    [Networked]
    private NetworkButtons _previousButtons { get; set; }

    public override void Spawned() {
      base.Spawned();

      Controller = GetComponent<NetworkCharacterController>();
      Debug.Log($"Player {Object.Id} spawned! HasStateAuthority: {HasStateAuthority}");

      if (!HasInputAuthority) return;
      Debug.Log($"Local player {Object.Id}: Setting up local player visuals and camera follow.");
      playerHead.ForEach(x => {
        if (x.TryGetComponent<MeshRenderer>(out var meshRenderer)) meshRenderer.enabled = false;
        if (x.TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer)) skinnedMeshRenderer.enabled = false;
      });

      FollowPlayer sceneFollowPlayer = FindFirstObjectByType<FollowPlayer>();
      if (sceneFollowPlayer != null) {
        sceneFollowPlayer.player = this;
        Debug.Log($"Local player {Object.Id} has set itself as the target for FollowPlayer '{sceneFollowPlayer.gameObject.name}'.");
      } else {
        Debug.LogError($"Local player {Object.Id} could not find a FollowPlayer instance in the scene. Camera will not follow.");
      }
    }

    public override void FixedUpdateNetwork() {
      if (!GetInput<NetInput>(out var input)) return;
      var inputDir = new Vector3(input.Direction.x, 0, input.Direction.y);
      var dir = input.GazeDirection * inputDir;
      dir.y = 0;
      // relativeTransform += dir.normalized * speed;

      if (Controller.Velocity.magnitude > 10) {
        Controller.Velocity = Vector3.zero;
      }
      Controller.Move(speed * dir.normalized * Runner.DeltaTime);

      if (gaze != null) {
        var inputGazePosition = input.GazePosition + relativeTransform;
        var inputGazePosition2d = new Vector2(inputGazePosition.x, inputGazePosition.z);
        gaze.transform.position = inputGazePosition + transform.position;
        gaze.transform.rotation = input.GazeDirection;
        if (inputGazePosition2d.magnitude > distanceBeforeCorrection) {
          Debug.DrawLine(gaze.transform.position, transform.position, Color.red);
          if (inputDir.magnitude < 0.9) {
            dir = inputGazePosition;
            dir.y = 0;
            Debug.DrawLine(gaze.transform.position, gaze.transform.position + dir, Color.yellow);
            var moveWithSpeedDir = dir.normalized * speed * 0.1f;
            Controller.Move(moveWithSpeedDir * Runner.DeltaTime);
            relativeTransform += -10 * moveWithSpeedDir * Runner.DeltaTime;
          }
        }
      }

      if (leftController != null) {
        leftController.transform.position = input.LeftControllerPosition + transform.position + relativeTransform;
        leftController.transform.rotation = input.LeftControllerRotation;
      }

      if (rightController != null) {
        rightController.transform.position = input.RightControllerPosition + transform.position + relativeTransform;
        rightController.transform.rotation = input.RightControllerRotation;
      }

      if (input.Buttons.WasPressed(_previousButtons, InputButton.Shoot)) {
        if (CurrentWeapon != null) {
          // Call Fire() from the correct network context
          CurrentWeapon.Fire();
        }
      }
      _previousButtons = input.Buttons;
    }
    public void SetCurrentWeapon(Weapon_ProjectileDataBuffer_Hitscan weapon) {
      Debug.Log($"Player setting current weapon: {(weapon != null ? weapon.name : "null")}");
      CurrentWeapon = weapon;
    }
  }

}
