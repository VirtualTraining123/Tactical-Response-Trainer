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
    
    [Header("Player Avatar Prefab")]
    [SerializeField] private GameObject playerPrefab; // Prefab del avatar que usará VR IK
    
    private AvatarMP avatarMP; // Referencia al script del avatar
    private Vector3 _relativeTransform = Vector3.zero;

    private void Start() {
    }

    public override void Spawned() {
      base.Spawned();
      Controller = gameObject.GetComponent<NetworkCharacterController>();
      Debug.Log("Player marker spawned!!");

      // Si es el que tiene autoridad de estado y se asignó el prefab
      if (HasStateAuthority && playerPrefab != null) {
          // Instanciar el avatar como hijo del marcador para facilitar el seguimiento
          GameObject playerAvatar = Instantiate(playerPrefab, transform.position, transform.rotation, transform);
          avatarMP = playerAvatar.GetComponent<AvatarMP>();
          if (avatarMP != null) {
              // Bindear las posiciones del player marker al avatar mediante sus setters
              avatarMP.SetLeftHandTarget(leftController.transform);
              avatarMP.SetRightHandTarget(rightController.transform);
              avatarMP.SetHeadTarget(gaze.transform);
          } else {
              Debug.LogWarning("No se encontró el componente AvatarMP en el prefab del avatar.");
          }
      }
    }

    public override void FixedUpdateNetwork() {
      if (!HasStateAuthority) return;
      if (!GetInput<NetInput>(out var input)) return;
      
      Debug.Log($"Player input: {input.Direction}");
      
      // Calcular la dirección a partir de la gaze y el input direccional
      var dir = input.GazeDirection * new Vector3(input.Direction.x, 0, input.Direction.y);
      dir.y = 0;
      _relativeTransform += dir.normalized * speed;
      var targetPosition = input.GazePosition + _relativeTransform;
      
      // Movimiento del player marker
      Controller.Move(speed * dir.normalized * Runner.DeltaTime);
      gaze.transform.localPosition = input.GazePosition - transform.position * 0.5f;
      gaze.transform.rotation = input.GazeDirection;
      leftController.transform.localPosition = input.LeftControllerPosition - transform.position * 0.5f;
      leftController.transform.rotation = input.LeftControllerRotation;
      rightController.transform.localPosition = input.RightControllerPosition - transform.position * 0.5f;
      rightController.transform.rotation = input.RightControllerRotation;
    }

    private void Shoot() {
      throw new System.NotImplementedException();
    }
  }
}
