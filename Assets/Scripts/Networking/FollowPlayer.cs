using JetBrains.Annotations;
using Unity.XR.CoreUtils;
using UnityEngine;

namespace Networking {
  public class FollowPlayer: MonoBehaviour {
    [CanBeNull] public NetworkedPlayer player;
    public XROrigin origin;

    public void Update() {
      if (!player) return;
      origin.MoveCameraToWorldLocation(player.gaze.transform.position);
      // transform.position = player.gaze.transform.position;
      // transform.rotation = player.gaze.transform.rotation;
    }
  }
}