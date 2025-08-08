using JetBrains.Annotations;
using Unity.XR.CoreUtils;
using UnityEngine;
using Fusion;

namespace Networking {
  public class FollowPlayer : MonoBehaviour {
    [CanBeNull] public NetworkedPlayer player;
    public XROrigin origin;

    private NetworkRunner _cachedRunner;

    private void Start() {
      _cachedRunner = FindFirstObjectByType<NetworkRunner>();
    }

    public void Update() {
      if (!_cachedRunner) {
        _cachedRunner = FindFirstObjectByType<NetworkRunner>();
        if (!_cachedRunner) {
          return;
        }
      }

      if (!player) {
        Debug.Log("FollowPlayer: No player assigned");
        return;
      }

      Debug.Log($"FollowPlayer: Following player {player.Object.Id}, HasInputAuthority: {player.Object.HasInputAuthority}, Position: {(player.gaze != null ? player.gaze.transform.position : Vector3.zero)}");

      if (!player.Object || !player.Object.IsValid) {
        Debug.LogWarning($"FollowPlayer: Player object {player.Object?.Id} is null or invalid. Clearing reference.");
        player = null;
        return;
      }

      if (!player.Object.HasInputAuthority) {
        Debug.Log($"FollowPlayer: Player {player.Object.Id} does not have input authority, not following");
        return;
      }

      if (player.gaze && origin) {
        origin.MoveCameraToWorldLocation(player.gaze.transform.position);
        Debug.Log($"FollowPlayer: Moving camera to {player.gaze.transform.position} for player {player.Object.Id}");
      } else {
        Debug.LogWarning($"FollowPlayer: Player {player.Object.Id} gaze is null or origin is not set");
      }
    }
  }
}
