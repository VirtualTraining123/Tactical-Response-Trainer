using Fusion;
using UnityEngine;

namespace Networking {
  public class NetworkGameLogic : NetworkBehaviour, IPlayerJoined, IPlayerLeft {
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [Networked, Capacity(12)] public NetworkDictionary<PlayerRef, NetworkedPlayer> Players => default;

    public bool isSpawned;

    public override void Spawned() {
      base.Spawned();
      isSpawned = true;
    }

    public void PlayerJoined(PlayerRef player) {
      if (!HasStateAuthority) return;
      var playerObject = Runner.Spawn(playerPrefab, Vector3.up, Quaternion.identity, player);
      Players.Add(player, playerObject.GetComponent<NetworkedPlayer>());
    }
  
    public void PlayerLeft(PlayerRef player) {
      if (!HasStateAuthority) return;
      if (!Players.TryGet(player, out var playerObject)) return;
      Runner.Despawn(playerObject.Object);
      Players.Remove(player);
    }
  }
}