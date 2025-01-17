using JetBrains.Annotations;
using UnityEngine;

namespace Networking {
  public class FollowPlayer: MonoBehaviour {
    [CanBeNull] public NetworkedPlayer player;

    public void Update() {
      if (!player) return;
      transform.position = player.transform.position;
      transform.rotation = player.transform.rotation;
    }
  }
}