using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
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

    [Header("Camera Rig")]
    [SerializeField] private XROrigin xrOrigin;
    [SerializeField] private float moveSpeed = 0.1f;
    [SerializeField] private float inputDeadzone = 0.1f;
    [SerializeField] private bool useCameraForward = true;

    private Vector3 _lastRigPosition;
    private bool _hasRigSnapshot;
    private ContinuousMoveProvider _continuousMoveProvider;
    private ContinuousTurnProvider _continuousTurnProvider;


    private void Awake()
    {
      if (xrOrigin == null)
      {
        xrOrigin = FindFirstObjectByType<XROrigin>();
      }

      if (xrOrigin != null)
      {
        _lastRigPosition = xrOrigin.transform.position;
        _hasRigSnapshot = true;
        DisableBuiltInLocomotion();
      }
      else
      {
        Debug.LogWarning("InputManager: No XROrigin assigned. Camera locomotion will be disabled until one is found.");
      }
    }

    public void BeforeUpdate()
    {
      if (_resetInput)
      {
        _accumulatedInput = default;
        _resetInput = false;
      }
      EnsureOriginReference();

      Vector2 xrInput = moveAction != null ? moveAction.ReadValue() : Vector2.zero;
      Vector2 keyboardInput = ReadKeyboardInput();

      Vector2 combinedInput = xrInput.sqrMagnitude > inputDeadzone * inputDeadzone ? xrInput : keyboardInput;
      if (combinedInput.sqrMagnitude < inputDeadzone * inputDeadzone)
      {
        combinedInput = Vector2.zero;
      }
      else
      {
        combinedInput = Vector2.ClampMagnitude(combinedInput, 1f);
      }

      _accumulatedInput.Direction = Flip(combinedInput);

      ApplyCameraLocomotion(combinedInput);

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
      input.Set(_accumulatedInput);
      _resetInput = true;
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner)
    {
    }

    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
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

    private void EnsureOriginReference()
    {
      if (xrOrigin != null) return;

      xrOrigin = FindFirstObjectByType<XROrigin>();
      if (xrOrigin != null)
      {
        _lastRigPosition = xrOrigin.transform.position;
        _hasRigSnapshot = true;
        Debug.Log("InputManager: Found XROrigin at runtime.");
        DisableBuiltInLocomotion();
      }
    }

    private Vector2 ReadKeyboardInput()
    {
      var keyboard = Keyboard.current;
      if (keyboard == null) return Vector2.zero;

      Vector2 keyboardInput = Vector2.zero;
      if (keyboard.wKey.isPressed) keyboardInput.y += 1f;
      if (keyboard.sKey.isPressed) keyboardInput.y -= 1f;
      if (keyboard.aKey.isPressed) keyboardInput.x -= 1f;
      if (keyboard.dKey.isPressed) keyboardInput.x += 1f;

      return keyboardInput.sqrMagnitude > 1f ? keyboardInput.normalized : keyboardInput;
    }

    private void ApplyCameraLocomotion(Vector2 movementInput)
    {
      if (xrOrigin == null) return;

      float magnitude = movementInput.sqrMagnitude;
      float deltaTime = Runner != null ? Runner.DeltaTime : Time.deltaTime;

      Transform rigTransform = xrOrigin.transform;
      Transform referenceTransform = useCameraForward && xrOrigin.Camera != null
        ? xrOrigin.Camera.transform
        : rigTransform;

      Vector3 forward = referenceTransform.forward;
      forward.y = 0f;
      forward.Normalize();

      Vector3 right = referenceTransform.right;
      right.y = 0f;
      right.Normalize();

      Vector3 planarMove = (right * movementInput.x + forward * movementInput.y) * moveSpeed * deltaTime;
      if (magnitude > 0f && planarMove.sqrMagnitude > float.Epsilon)
      {
        rigTransform.position += planarMove;
      }

      Vector3 currentRigPosition = rigTransform.position;
      if (!_hasRigSnapshot)
      {
        _lastRigPosition = currentRigPosition;
        _hasRigSnapshot = true;
      }

      Vector3 rigDelta = currentRigPosition - _lastRigPosition;
      _accumulatedInput.RigWorldPosition = currentRigPosition;
      _accumulatedInput.RigDelta = rigDelta;
      _accumulatedInput.RigWorldRotation = rigTransform.rotation;

      _lastRigPosition = currentRigPosition;
    }

    private void DisableBuiltInLocomotion()
    {
      if (xrOrigin == null) return;

      if (_continuousMoveProvider == null)
      {
        _continuousMoveProvider = xrOrigin.GetComponentInChildren<ContinuousMoveProvider>();
      }

      if (_continuousMoveProvider != null && _continuousMoveProvider.enabled)
      {
        _continuousMoveProvider.enabled = false;
        Debug.Log("InputManager: Disabled ContinuousMoveProvider to prevent double locomotion.");
      }

      if (_continuousTurnProvider == null)
      {
        _continuousTurnProvider = xrOrigin.GetComponentInChildren<ContinuousTurnProvider>();
      }

      if (_continuousTurnProvider != null && _continuousTurnProvider.enabled)
      {
        _continuousTurnProvider.enabled = false;
        Debug.Log("InputManager: Disabled ContinuousTurnProvider; use Network rotation instead.");
      }
    }
  }
}