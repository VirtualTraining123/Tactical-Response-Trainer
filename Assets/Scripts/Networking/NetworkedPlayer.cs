using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Projectiles;

namespace Networking
{
  [RequireComponent(typeof(NetworkCharacterController))]
  public class NetworkedPlayer : NetworkBehaviour
  {
    [Networked] private NetworkCharacterController Controller { get; set; }

    [Header("Player Marker Transforms")]
    [SerializeField]
    public GameObject leftController;

    [SerializeField] public GameObject rightController;
    [SerializeField] public GameObject gaze;
    [SerializeField] public List<GameObject> playerHead;
    [SerializeField] private Vector3 anchorOffset = Vector3.zero;

    [Header("Movement Settings")]
    [SerializeField] private float maxFollowDistance = 0.4f;
    [SerializeField] private float followLerpSpeed = 2f;
    [SerializeField] private float rotationFollowSpeed = 12f;
    [SerializeField] private float defaultPlayerHeight = 1.7f; // Default standing height
    [SerializeField] private float minPlayerHeight = 0.5f; // Minimum crouching height
    [SerializeField] private float groundRaycastDistance = 10f; // How far down to check for ground
    private readonly RaycastHit[] _groundProbeHits = new RaycastHit[8];
    public Weapon_ProjectileDataBuffer_Hitscan CurrentWeapon { get; set; }

    [Networked]
    private NetworkButtons _previousButtons { get; set; }

    private CapsuleCollider _capsuleCollider;

    public override void Spawned()
    {
      base.Spawned();

      Controller = GetComponent<NetworkCharacterController>();
      _capsuleCollider = GetComponent<CapsuleCollider>();

      if (_capsuleCollider == null)
      {
        _capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        _capsuleCollider.height = defaultPlayerHeight;
        _capsuleCollider.radius = 0.3f;
        _capsuleCollider.center = new Vector3(0, defaultPlayerHeight / 2f, 0);
      }

      Debug.Log($"Player {Object.Id} spawned. HasStateAuthority: {HasStateAuthority}");

      if (!HasInputAuthority) return;
      Debug.Log($"Local player {Object.Id}: Initializing local-only visuals and rig binding.");
      playerHead.ForEach(x =>
      {
        if (x.TryGetComponent<MeshRenderer>(out var meshRenderer)) meshRenderer.enabled = false;
        if (x.TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer)) skinnedMeshRenderer.enabled = false;
      });

      FollowPlayer sceneFollowPlayer = FindFirstObjectByType<FollowPlayer>();
      if (sceneFollowPlayer != null)
      {
        sceneFollowPlayer.player = this;
        Debug.Log($"Local player {Object.Id} registered with FollowPlayer '{sceneFollowPlayer.gameObject.name}'.");

        // CRITICAL: Move XROrigin to player spawn position so camera follows the player
        if (sceneFollowPlayer.origin != null)
        {
          // Teleport the XROrigin to the player's spawn position
          // This moves the VR camera rig to where the player spawned
          sceneFollowPlayer.origin.transform.position = transform.position;
          Debug.Log($"Teleported XROrigin to player spawn position: {transform.position}");
        }
        else
        {
          Debug.LogWarning("FollowPlayer found but has no XROrigin reference!");
        }
      }
      else
      {
        Debug.LogWarning($"Local player {Object.Id} could not find a FollowPlayer instance; ensure InputManager has an XROrigin reference.");
      }

    }

    public override void FixedUpdateNetwork()
    {
      if (!GetInput<NetInput>(out var input)) return;

      var runnerDeltaTime = Runner != null ? Runner.DeltaTime : Time.deltaTime;

      // Get the camera/headset height from the gaze position
      float headHeight = input.GazePosition.y;
      headHeight = Mathf.Clamp(headHeight, minPlayerHeight, defaultPlayerHeight + 0.5f);

      // Update capsule collider based on player height (crouch/stand)
      if (_capsuleCollider != null)
      {
        _capsuleCollider.height = Mathf.Max(headHeight, minPlayerHeight);
        _capsuleCollider.center = new Vector3(0, _capsuleCollider.height / 2f, 0);
      }

      Vector3 currentPosition = transform.position;

      // Calculate target position based on headset XZ position
      // The body should be directly below the headset
      Vector3 headWorldPos = input.RigWorldPosition + input.GazePosition;
      Vector3 targetPosition = new Vector3(headWorldPos.x, currentPosition.y, headWorldPos.z);

      Vector3 toTarget = targetPosition - currentPosition;
      Vector3 planarToTarget = new Vector3(toTarget.x, 0f, toTarget.z);
      float planarSqrMagnitude = planarToTarget.sqrMagnitude;
      float maxErrorSqr = maxFollowDistance * maxFollowDistance;

      Vector3 nextPosition = currentPosition;
      if (planarSqrMagnitude > 0.000001f)
      {
        if (planarSqrMagnitude > maxErrorSqr)
        {
          nextPosition.x = targetPosition.x;
          nextPosition.z = targetPosition.z;
        }
        else
        {
          float maxStep = Mathf.Max(followLerpSpeed * runnerDeltaTime, 0.0001f);
          nextPosition = Vector3.MoveTowards(currentPosition, targetPosition, maxStep);
        }
      }

      // Simple ground detection - always keep feet on ground
      // Cast ray downward from the target XZ position to find ground
      Vector3 rayOrigin = new Vector3(nextPosition.x, currentPosition.y + 2f, nextPosition.z);
      int hitCount = Physics.RaycastNonAlloc(rayOrigin, Vector3.down, _groundProbeHits, groundRaycastDistance,
        Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

      float groundY = currentPosition.y; // Default to current Y if no ground found
      float closestGroundDistance = float.MaxValue;

      for (int i = 0; i < hitCount; i++)
      {
        var hit = _groundProbeHits[i];
        if (hit.transform.IsChildOf(transform))
        {
          continue;
        }

        if (hit.distance < closestGroundDistance)
        {
          closestGroundDistance = hit.distance;
          groundY = hit.point.y;
        }
      }

      // Always place feet on the ground
      nextPosition.y = groundY;

      // Only teleport if position changed
      if (Vector3.Distance(currentPosition, nextPosition) > 0.001f)
      {
        Controller.Teleport(nextPosition);
      }

      Quaternion targetYaw = Quaternion.Euler(0f, input.RigWorldRotation.eulerAngles.y, 0f);
      transform.rotation = Quaternion.Slerp(transform.rotation, targetYaw, rotationFollowSpeed * runnerDeltaTime);

      // Position tracked objects relative to player body on the ground
      // Body XZ position is already at the headset's XZ location (calculated from headWorldPos)
      // So we only need to add vertical offset for camera height
      Vector3 bodyPosition = transform.position;

      if (gaze != null)
      {
        // Camera is simply at body position + vertical head height
        // Body XZ is already positioned under the head, so no lateral offset needed
        Vector3 cameraPosition = bodyPosition + Vector3.up * headHeight;
        gaze.transform.SetPositionAndRotation(cameraPosition, input.GazeDirection);
      }

      if (leftController != null)
      {
        // Controllers use world position from rig + their offset
        Vector3 leftControllerWorldPos = input.RigWorldPosition + input.LeftControllerPosition;
        // Adjust Y to be relative to body on ground instead of rig
        leftControllerWorldPos.y = bodyPosition.y + input.LeftControllerPosition.y;
        leftController.transform.SetPositionAndRotation(leftControllerWorldPos, input.LeftControllerRotation);
      }

      if (rightController != null)
      {
        // Controllers use world position from rig + their offset
        Vector3 rightControllerWorldPos = input.RigWorldPosition + input.RightControllerPosition;
        // Adjust Y to be relative to body on ground instead of rig
        rightControllerWorldPos.y = bodyPosition.y + input.RightControllerPosition.y;
        rightController.transform.SetPositionAndRotation(rightControllerWorldPos, input.RightControllerRotation);
      }

      if (input.Buttons.WasPressed(_previousButtons, InputButton.Shoot))
      {
        if (CurrentWeapon != null)
        {
          CurrentWeapon.Fire();
        }
      }
      _previousButtons = input.Buttons;
    }
    public void SetCurrentWeapon(Weapon_ProjectileDataBuffer_Hitscan weapon)
    {
      Debug.Log($"Player setting current weapon: {(weapon != null ? weapon.name : "null")}");
      CurrentWeapon = weapon;
    }
  }

}
