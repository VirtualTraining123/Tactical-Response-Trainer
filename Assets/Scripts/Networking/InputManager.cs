using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;

namespace Networking {
  public class InputManager : SimulationBehaviour, IBeforeUpdate, INetworkRunnerCallbacks {
    private NetInput _accumulatedInput;
    private bool _resetInput;

    [SerializeField] public bool invertX = true;
    [SerializeField] public bool invertY = true;
    [SerializeField] public XRInputValueReader<float> shootAction;
    [SerializeField] public XRInputValueReader<Vector2> moveAction;
    [SerializeField] public XRInputValueReader<Quaternion> gazeRotation;
    [SerializeField] public XRInputValueReader<Vector3> gazePosition;
    [SerializeField] public XRInputValueReader<Quaternion> leftControllerRotation;
    [SerializeField] public XRInputValueReader<Vector3> leftControllerPosition;
    [SerializeField] public XRInputValueReader<Quaternion> rightControllerRotation;
    [SerializeField] public XRInputValueReader<Vector3> rightControllerPosition;


    private void Awake() {
      Debug.Log("InputManager Awake");
    }

    public void BeforeUpdate() {
      if (_resetInput) {
        _accumulatedInput = default;
        _resetInput = false;
      }

      if (shootAction != null){
        float shootValue = shootAction.ReadValue();
        bool isShooting = shootValue > 0.5f;
        _accumulatedInput.Buttons.Set(InputButton.Shoot, isShooting); 
      }
      if (moveAction != null) _accumulatedInput.Direction = Flip(moveAction.ReadValue().normalized);
      if (gazeRotation != null) _accumulatedInput.GazeDirection = MapRotation(gazeRotation.ReadValue());
      if (gazePosition != null) _accumulatedInput.GazePosition = MapPosition(gazePosition.ReadValue());
      if (leftControllerRotation != null)
        _accumulatedInput.LeftControllerRotation = MapRotation(leftControllerRotation.ReadValue());
      if (leftControllerPosition != null)
        _accumulatedInput.LeftControllerPosition = MapPosition(leftControllerPosition.ReadValue());
      if (rightControllerRotation != null)
        _accumulatedInput.RightControllerRotation = MapRotation(rightControllerRotation.ReadValue());
      if (rightControllerPosition != null)
        _accumulatedInput.RightControllerPosition = MapPosition(rightControllerPosition.ReadValue());
    }
    
    private Vector2 Flip(Vector2 vector) {
      return new Vector2(vector.x * (invertX ? -1 : 1), vector.y * (invertY ? -1 : 1));
    }

    private static Vector3 MapPosition(Vector3 position) {
      return new Vector3(-position.x, position.y, -position.z);
    }

    private static Quaternion MapRotation(Quaternion rotation) {
      return new Quaternion(rotation.x, -rotation.y, rotation.z, -rotation.w);
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) {
      _accumulatedInput.Direction.Normalize();

      bool isShootSet = _accumulatedInput.Buttons.IsSet(InputButton.Shoot);
      Debug.Log($"InputManager.OnInput: Sending Input. Shoot Button State: {isShootSet}");
      input.Set(_accumulatedInput);
      _resetInput = true;
      if (_accumulatedInput.Direction.magnitude > 0.01) {
        Debug.Log(_accumulatedInput.Direction);
      }
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {
    }

    public void OnConnectedToServer(NetworkRunner runner) {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,
      ArraySegment<byte> data) {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {
    }

    public void OnSceneLoadDone(NetworkRunner runner) {
    }

    public void OnSceneLoadStart(NetworkRunner runner) {
    }
  }
}