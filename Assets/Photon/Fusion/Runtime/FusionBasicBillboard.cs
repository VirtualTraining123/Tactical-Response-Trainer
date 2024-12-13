using UnityEngine;

namespace Fusion {
  /// <summary>
  ///   Component which automatically faces this GameObject toward the supplied Camera. If Camera == null, will face towards
  ///   Camera.main.
  /// </summary>
  [ScriptHelp(BackColor = ScriptHeaderBackColor.Olive)]
  [ExecuteAlways]
  public class FusionBasicBillboard : Behaviour {
    // Camera find is expensive, so do it once per update for ALL implementations
    private static float _lastCameraFindTime;
    private static Camera _currentCam;

    /// <summary>
    ///   Force a particular camera to billboard this object toward. Leave null to use Camera.main.
    /// </summary>
    [InlineHelp] public Camera Camera;

    private Camera MainCamera {
      set => _currentCam = value;
      get {
        var time = Time.time;
        // Only look for the camera once per Update.
        if (time == _lastCameraFindTime)
          return _currentCam;

        _lastCameraFindTime = time;
        var cam = Camera.main;
        _currentCam = cam;
        return cam;
      }
    }

    private void LateUpdate() {
      UpdateLookAt();
    }

    private void OnEnable() {
      UpdateLookAt();
    }

    private void OnDisable() {
      transform.localRotation = default;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
      LateUpdate();
    }
#endif

    public void UpdateLookAt() {
      var cam = Camera ? Camera : MainCamera;

      if (cam)
        if (enabled)
          transform.rotation = cam.transform.rotation;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() {
      _currentCam = default;
      _lastCameraFindTime = default;
    }
  }
}