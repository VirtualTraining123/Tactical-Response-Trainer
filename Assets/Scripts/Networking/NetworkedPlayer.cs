using System;
using Fusion;
using UnityEngine;

namespace Networking {
  [RequireComponent(typeof(NetworkCharacterController))]
  public class NetworkedPlayer : NetworkBehaviour {
    [Networked] private NetworkCharacterController Controller { get; set; }
    
    [Header("Player Marker Transforms")]
    [SerializeField] public GameObject leftController;
    [SerializeField] public GameObject rightController;
    [SerializeField] public GameObject gaze;
    
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    private Vector3 _relativeTransform = Vector3.zero;
    
    [Header("Avatar Multiplayer")]
    [SerializeField] private GameObject avatarPrefab; // Prefab del avatar multiplayer a instanciar dinámicamente.
    private AvatarMp avatarMp; // Referencia al script AvatarMp en el avatar.
    
    private void Start() {
      // Aquí podrías inicializar otros elementos si es necesario.
    }
    
    public override void Spawned() {
      base.Spawned();
      
      Controller = GetComponent<NetworkCharacterController>();
      Debug.Log("Player spawned!!");
      
      // Instanciar dinámicamente el avatar si se asignó un prefab.
      if (avatarPrefab != null) {
        // Se instancia el avatar como hijo del PlayerMaker para que se sincronice la posición.
        GameObject avatarInstance = Instantiate(avatarPrefab, transform.position, transform.rotation, transform);
        avatarMp = avatarInstance.GetComponent<AvatarMp>();
        if (avatarMp != null) {
          // Asignar las referencias de los targets del PlayerMarker al avatar para el VR IK.
          avatarMp.SetLeftHandTarget(leftController.transform);
          avatarMp.SetRightHandTarget(rightController.transform);
          avatarMp.SetHeadTarget(gaze.transform);
        } else {
          Debug.LogWarning("No se encontró el componente AvatarMp en el prefab del avatar.");
        }
      } else {
        Debug.LogWarning("AvatarPrefab no asignado en NetworkedPlayer.");
      }
    }
    
    public override void FixedUpdateNetwork() {
      if (!HasStateAuthority) return;
      if (!GetInput<NetInput>(out var input)) return;
      
      Debug.Log($"Player input: {input.Direction}");
      
      // Calcular la dirección de movimiento en base a la dirección de la mirada y el input.
      var dir = input.GazeDirection * new Vector3(input.Direction.x, 0, input.Direction.y);
      dir.y = 0;
      _relativeTransform += dir.normalized * speed;
      var targetPosition = input.GazePosition + _relativeTransform;
      
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
    
    private void Shoot() {
      throw new System.NotImplementedException();
    }
  }
}
