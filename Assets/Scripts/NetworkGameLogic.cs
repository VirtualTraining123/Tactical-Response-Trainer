using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkGameLogic : NetworkBehaviour, IPlayerJoined, IPlayerLeft {
  [SerializeField] private NetworkPrefabRef playerPrefab;
  [Networked, Capacity(12)] public NetworkDictionary<PlayerRef, NetworkedPlayer> Players => default;

  [DoNotSerialize] public bool IsSpawned = false;

  public override void Spawned() {
    base.Spawned();
    IsSpawned = true;
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