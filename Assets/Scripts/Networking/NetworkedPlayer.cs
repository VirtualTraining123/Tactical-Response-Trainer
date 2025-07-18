using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Projectiles;
using Unity.XR.CoreUtils;

namespace Networking
{
  [RequireComponent(typeof(NetworkCharacterController))]
  public class NetworkedPlayer : NetworkBehaviour
  {
    [Header("XR Setup")]
    [SerializeField] private GameObject xrRigRoot;
    [Networked] private NetworkCharacterController Controller { get; set; }

    [Header("Player Marker Transforms")]
    [SerializeField]
    public GameObject leftController;

    [SerializeField] public GameObject rightController;
    [SerializeField] public GameObject gaze;
    [SerializeField] public List<GameObject> playerHead;

    [Header("Movement Settings")]
    [SerializeField]
    private float speed = 5f;

    [HideInInspector]
    [Networked]
    public Weapon_ProjectileDataBuffer_Hitscan CurrentWeapon { get; set; }

    [Networked]
    private NetworkButtons _previousButtons { get; set; }

    [Networked] private Vector3 NetworkedGazePos { get; set; }
    [Networked] private Quaternion NetworkedGazeRot { get; set; }
    [Networked] private Vector3 NetworkedLeftCtrlPos { get; set; }
    [Networked] private Quaternion NetworkedLeftCtrlRot { get; set; }
    [Networked] private Vector3 NetworkedRightCtrlPos { get; set; }
    [Networked] private Quaternion NetworkedRightCtrlRot { get; set; }

    private void Start()
    {
      Debug.Log($"NetworkedPlayer Start - Object: {Object?.Id}, HasInputAuthority: {Object?.HasInputAuthority}");
    }

    public override void Spawned()
    {
      base.Spawned();

      Controller = GetComponent<NetworkCharacterController>();
      Debug.Log($"Player {Object.Id} spawned! HasStateAuthority: {HasStateAuthority}, HasInputAuthority: {Object.HasInputAuthority}");

      GameObject uiCamObject = GameObject.Find("UI Camera");
      if (uiCamObject != null)
      {
        uiCamObject.SetActive(false);
      }

      if (Object.HasInputAuthority)
      {
        Debug.Log($"LOCAL player {Object.Id}: Setting up local player visuals and camera follow.");
        playerHead.ForEach(x =>
        {
          if (x.TryGetComponent<MeshRenderer>(out var meshRenderer)) meshRenderer.enabled = false;
          if (x.TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer)) skinnedMeshRenderer.enabled = false;
        });

        if (xrRigRoot != null) xrRigRoot.SetActive(true);
        else Debug.LogWarning($"xrRigRoot is null on local player {Object.Id}.");

        FollowPlayer sceneFollowPlayer = FindFirstObjectByType<FollowPlayer>();
        if (sceneFollowPlayer != null)
        {
          sceneFollowPlayer.player = this;
          Debug.Log($"LOCAL player {Object.Id}: Set itself as target for FollowPlayer.");
        }
        else Debug.LogError($"LOCAL player {Object.Id} could not find a FollowPlayer instance.");
      }
      else
      {
        Debug.Log($"REMOTE player {Object.Id}: Setting up remote player visuals.");
        if (xrRigRoot != null) xrRigRoot.SetActive(false);
        playerHead.ForEach(x =>
        {
          if (x.TryGetComponent<MeshRenderer>(out var meshRenderer)) meshRenderer.enabled = true;
          if (x.TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer)) skinnedMeshRenderer.enabled = true;
        });
      }
    }

    public override void FixedUpdateNetwork()
    {
      // Only host runs this logic
      if (HasStateAuthority && GetInput<NetInput>(out var input))
      {
        var dir = input.GazeDirection * new Vector3(input.Direction.x, 0, input.Direction.y);
        dir.y = 0;
        Controller.Move(speed * dir.normalized * Runner.DeltaTime);

        NetworkedGazePos = input.GazePosition;
        NetworkedGazeRot = input.GazeDirection;
        NetworkedLeftCtrlPos = input.LeftControllerPosition;
        NetworkedLeftCtrlRot = input.LeftControllerRotation;
        NetworkedRightCtrlPos = input.RightControllerPosition;
        NetworkedRightCtrlRot = input.RightControllerRotation;

        if (input.Buttons.WasPressed(_previousButtons, InputButton.Shoot))
        {
          if (CurrentWeapon != null)
          {
            CurrentWeapon.Fire();
          }
        }
        _previousButtons = input.Buttons;
      }
    }

    public override void Render()
    {
      if (gaze != null)
      {
        gaze.transform.position = transform.position + NetworkedGazePos;
        gaze.transform.rotation = NetworkedGazeRot;
      }

      if (leftController != null)
      {
        leftController.transform.position = transform.position + NetworkedLeftCtrlPos;
        leftController.transform.rotation = NetworkedLeftCtrlRot;
      }

      if (rightController != null)
      {
        rightController.transform.position = transform.position + NetworkedRightCtrlPos;
        rightController.transform.rotation = NetworkedRightCtrlRot;
      }
    }

    public void SetCurrentWeapon(Weapon_ProjectileDataBuffer_Hitscan weapon)
    {
      Debug.Log($"Player setting current weapon: {(weapon != null ? weapon.name : "null")}");
      CurrentWeapon = weapon;
    }
  }
}