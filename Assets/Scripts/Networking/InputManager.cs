using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.InputSystem;

namespace Networking
{
  public class InputManager : SimulationBehaviour, IBeforeUpdate, INetworkRunnerCallbacks
  {
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


    private void Awake()
    {
      Debug.Log("InputManager Awake");
    }

    public void BeforeUpdate()
    {
      if (_resetInput)
      {
        _accumulatedInput = default;
        _resetInput = false;
      }
      Vector2 xrInput = Vector2.zero;
      if (moveAction != null)
      {
        Vector2 rawMoveInput = moveAction.ReadValue();
        Vector2 flippedInput = Flip(rawMoveInput.normalized);
        _accumulatedInput.Direction = flippedInput;
        // Log the movement input to see what's happening
        if (Time.frameCount % 120 == 0)
        {
          Debug.Log($"InputManager: Raw move input: {rawMoveInput}, Flipped: {flippedInput}");
        }
      }
      else
      {
        Debug.LogWarning("InputManager: moveAction is null!");
      }

      // TODO: remove this,. emporary keyboard fallback for testing movement logic
      Vector2 keyboardInput = Vector2.zero;
      var keyboard = Keyboard.current;
      if (keyboard != null)
      {
        Debug.Log("InputManager: Keyboard is available, checking keys.");
        if (keyboard.wKey.isPressed) keyboardInput.y += 1;
        if (keyboard.sKey.isPressed) keyboardInput.y -= 1;
        if (keyboard.aKey.isPressed) keyboardInput.x -= 1;
        if (keyboard.dKey.isPressed) keyboardInput.x += 1;
        if (keyboardInput.magnitude > 0.01f && Time.frameCount % 120 == 0)
        {
          Debug.Log($"InputManager: Keyboard input detected: {keyboardInput}");
        }
      }

      // Use keyboard input if XR input is zero, otherwise use XR input
      Vector2 finalInput = xrInput.magnitude > 0.01f ? xrInput : keyboardInput;
      _accumulatedInput.Direction = Flip(finalInput.normalized);

      if (finalInput.magnitude > 0.01f && Time.frameCount % 120 == 0)
      {
        Debug.Log($"InputManager: Final input used: {finalInput}, Flipped: {_accumulatedInput.Direction}");
      }


      if (shootAction != null)
      {
        float shootValue = shootAction.ReadValue();
        bool isShooting = shootValue > 0.5f;
        _accumulatedInput.Buttons.Set(InputButton.Shoot, isShooting);
      }
      //if (moveAction != null) _accumulatedInput.Direction = Flip(moveAction.ReadValue().normalized);
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

    private Vector2 Flip(Vector2 vector)
    {
      return new Vector2(vector.x * (invertX ? -1 : 1), vector.y * (invertY ? -1 : 1));
    }

    private static Vector3 MapPosition(Vector3 position)
    {
      return new Vector3(-position.x, position.y, -position.z);
    }

    private static Quaternion MapRotation(Quaternion rotation)
    {
      return new Quaternion(rotation.x, -rotation.y, rotation.z, -rotation.w);
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
      Debug.Log("Player joined: " + player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
      //Debug.Log("OnInput!!!");
      _accumulatedInput.Direction.Normalize();
      input.Set(_accumulatedInput);
      _resetInput = true;
      if (_accumulatedInput.Direction.magnitude > 0.01)
      {
        Debug.Log(_accumulatedInput.Direction);
      }
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,
      ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
  }
}