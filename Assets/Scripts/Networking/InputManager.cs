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
    [SerializeField] public XRInputValueReader<bool> shootAction;
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

      if (shootAction != null && shootAction.ReadValue()) {
        _accumulatedInput.Buttons.SetDown(InputButton.Shoot);
      }

      if (moveAction != null) {
        _accumulatedInput.Direction += moveAction.ReadValue().normalized;
      }
      
      if (gazeRotation != null) {
        _accumulatedInput.GazeDirection = gazeRotation.ReadValue();
      }
      
      if (gazePosition != null) {
        _accumulatedInput.GazePosition = gazePosition.ReadValue();
      }
      
      if (leftControllerRotation != null) {
        _accumulatedInput.LeftControllerRotation = leftControllerRotation.ReadValue();
      }
      
      if (leftControllerPosition != null) {
        _accumulatedInput.LeftControllerPosition = leftControllerPosition.ReadValue();
      }
      
      if (rightControllerRotation != null) {
        _accumulatedInput.RightControllerRotation = rightControllerRotation.ReadValue();
      }
      
      if (rightControllerPosition != null) {
        _accumulatedInput.RightControllerPosition = rightControllerPosition.ReadValue();
      }
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
      Debug.Log("OnInput!!!");
      _accumulatedInput.Direction.Normalize();
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