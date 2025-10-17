using JetBrains.Annotations;
using Unity.XR.CoreUtils;
using UnityEngine;
using Fusion;

namespace Networking
{
  public class FollowPlayer : MonoBehaviour
  {
    [CanBeNull] public NetworkedPlayer player;
    public XROrigin origin;

    private NetworkRunner _cachedRunner;
    private bool _loggedMissingOrigin;

    private void Start()
    {
      _cachedRunner = FindFirstObjectByType<NetworkRunner>();
    }

    public void Update()
    {
      if (!_cachedRunner)
      {
        _cachedRunner = FindFirstObjectByType<NetworkRunner>();
        if (!_cachedRunner)
        {
          return;
        }
      }

      if (!player)
      {
        return;
      }

      if (!player.Object || !player.Object.IsValid)
      {
        player = null;
        return;
      }

      if (!player.Object.HasInputAuthority)
      {
        return;
      }

      if (origin == null)
      {
        if (!_loggedMissingOrigin)
        {
          Debug.LogWarning("FollowPlayer: No XROrigin assigned; InputManager cannot drive the local rig without one.");
          _loggedMissingOrigin = true;
        }
        return;
      }

      // Camera rig movement is handled by InputManager. This script now simply keeps references alive.
    }
  }
}
