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

    private Vector3 relativeTransform = Vector3.zero;
    [HideInInspector]
    [Networked]
    public Weapon_ProjectileDataBuffer_Hitscan CurrentWeapon { get; set; }
    [Networked]
    private NetworkButtons _previousButtons { get; set; }

    public override void Spawned()
    {
      base.Spawned();

      Controller = GetComponent<NetworkCharacterController>();
      Debug.Log("Player spawned!!");
      GameObject uiCamObject = GameObject.Find("UI Camera"); // Find the camera by name
      Debug.Log($"uiCamObject: {uiCamObject}");
      if (uiCamObject != null)
      {
        uiCamObject.SetActive(false); // Deactivate the camera
      }
      if (HasStateAuthority)
      {
        playerHead.ForEach(x =>
        {
          if (x.TryGetComponent<MeshRenderer>(out var meshRenderer)) meshRenderer.enabled = false;
          if (x.TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer)) skinnedMeshRenderer.enabled = false;
        });
        if (xrRigRoot != null) xrRigRoot.SetActive(true);
      }
      else
      {
        Debug.Log($"Deactivating XR Rig for remote player {Object.Id}");
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
      if (!HasStateAuthority) return;
      if (!GetInput<NetInput>(out var input)) return;

      // Debug.Log($"Player input: {input.Direction}");

      // Calcular la dirección de movimiento en base a la dirección de la mirada y el input.
      var dir = input.GazeDirection * new Vector3(input.Direction.x, 0, input.Direction.y);
      dir.y = 0;
      relativeTransform += dir.normalized * speed;
      // var targetPosition = input.GazePosition + relativeTransform;

      // Mover el PlayerMaker (marca de jugador)
      if (Controller.Velocity.magnitude > 10)
      {
        Controller.Velocity = Vector3.zero;
      }
      Controller.Move(speed * dir.normalized * Runner.DeltaTime);

      // Actualizar las posiciones y rotaciones de los elementos del PlayerMarker
      if (gaze != null)
      {
        gaze.transform.position = input.GazePosition + transform.position;
        gaze.transform.rotation = input.GazeDirection;
      }

      if (leftController != null)
      {
        leftController.transform.position = input.LeftControllerPosition + transform.position;
        leftController.transform.rotation = input.LeftControllerRotation;
      }

      if (rightController != null)
      {
        rightController.transform.position = input.RightControllerPosition + transform.position;
        rightController.transform.rotation = input.RightControllerRotation;
      }

      if (input.Buttons.WasPressed(_previousButtons, InputButton.Shoot))
      {
        if (CurrentWeapon != null)
        {
          // Call Fire() from the correct network context
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