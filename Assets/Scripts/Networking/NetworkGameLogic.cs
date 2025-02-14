using Fusion;
using UnityEngine;

namespace Networking {
  public class NetworkGameLogic : NetworkBehaviour, IPlayerJoined, IPlayerLeft {
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private FollowPlayer followPlayer;
    [Networked, Capacity(12)] public NetworkDictionary<PlayerRef, NetworkedPlayer> Players => default;

    public bool isSpawned;

    public override void Spawned() {
      base.Spawned();
      isSpawned = true;
    }

    public void PlayerJoined(PlayerRef player) {
      if (!HasStateAuthority) return;
      var playerObject = Runner.Spawn(playerPrefab, Vector3.up, Quaternion.identity, player);
      var networkedPlayer = playerObject.GetComponent<NetworkedPlayer>();
      followPlayer.player = networkedPlayer;
      Players.Add(player, networkedPlayer);
    }
  
    public void PlayerLeft(PlayerRef player) {
      if (!HasStateAuthority) return;
      if (!Players.TryGet(player, out var playerObject)) return;
      Runner.Despawn(playerObject.Object);
      Players.Remove(player);
    }
  }
}