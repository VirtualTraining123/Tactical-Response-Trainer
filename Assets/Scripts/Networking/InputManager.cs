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


    private void Awake() {
      Debug.Log("InputManager Awake");
    }

    public void BeforeUpdate() {
      // throw new NotImplementedException();
      if (_resetInput) {
        _accumulatedInput = default;
        _resetInput = false;
      }

      if (shootAction != null && shootAction.ReadValue()) {
        // Debug.Log(shootAction.ReadValue());
        _accumulatedInput.Buttons.SetDown(InputButton.Shoot);
      }

      
      if (moveAction != null) {
        // Debug.Log(moveAction.ReadValue());
        _accumulatedInput.Direction += moveAction.ReadValue().normalized;
      }
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {
      // throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {
      // throw new NotImplementedException();
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
      // throw new NotImplementedException();
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
      // throw new NotImplementedException();
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) {
      // throw new NotImplementedException();
      Debug.Log("OnInput!!!");
      _accumulatedInput.Direction.Normalize();
      _accumulatedInput.LookDirection = Quaternion.identity;
      input.Set(_accumulatedInput);
      _resetInput = true;
      if (_accumulatedInput.Direction.magnitude > 0.01) {
        Debug.Log(_accumulatedInput.Direction);
      }
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {
      // throw new NotImplementedException();
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {
      // throw new NotImplementedException();
    }

    public void OnConnectedToServer(NetworkRunner runner) {
      // throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {
      // throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {
      // throw new NotImplementedException();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {
      // throw new NotImplementedException();
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {
      // throw new NotImplementedException();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
      // throw new NotImplementedException();
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {
      // throw new NotImplementedException();
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {
      // throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,
      ArraySegment<byte> data) {
      // throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {
      // throw new NotImplementedException();
    }

    public void OnSceneLoadDone(NetworkRunner runner) {
      // throw new NotImplementedException();
    }

    public void OnSceneLoadStart(NetworkRunner runner) {
      // throw new NotImplementedException();
    }
  }
}