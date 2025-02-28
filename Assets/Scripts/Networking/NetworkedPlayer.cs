using System;
using Fusion;
using UnityEngine;

namespace Networking {
  [RequireComponent(typeof(NetworkCharacterController))]
  public class NetworkedPlayer : NetworkBehaviour {
    [Networked] private NetworkCharacterController Controller { get; set; }
    [SerializeField] public GameObject leftController;
    [SerializeField] public GameObject rightController;
    [SerializeField] public GameObject gaze;
    [SerializeField] private float speed = 5f;
    private Vector3 _relativeTransform = Vector3.zero;


    private void Start() {
    }

    public override void Spawned() {
      base.Spawned();
      Controller = gameObject.GetComponent<NetworkCharacterController>();
      Debug.Log("Player spawned!!");
    }

    public override void FixedUpdateNetwork() {
      if (!HasStateAuthority) return;
      if (!GetInput<NetInput>(out var input)) return;
      //Debug.Log($"Player input: {input.Direction}");
      var dir = input.GazeDirection * new Vector3(input.Direction.x, 0, input.Direction.y);
      dir.y = 0;
      _relativeTransform += dir.normalized * speed;
      var targetPosition = input.GazePosition + _relativeTransform;
      // transform.position = targetPosition;
      // // Shoot if the shoot button is pressed
      // if (input.Buttons.IsSet(InputButton.Shoot)) {
      //   Shoot();
      // }
      
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