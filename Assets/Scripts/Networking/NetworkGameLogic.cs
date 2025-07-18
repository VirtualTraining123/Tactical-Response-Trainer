using Fusion;
using UnityEngine;

namespace Networking
{
  public class NetworkGameLogic : NetworkBehaviour, IPlayerJoined, IPlayerLeft
  {
    [Header("Prefabs")]
    [SerializeField]
    private NetworkPrefabRef playerPrefab;

    [Networked, Capacity(12)] public NetworkDictionary<PlayerRef, NetworkedPlayer> Players => default;

    public bool isSpawned;

    public override void Spawned()
    {
      base.Spawned();
      isSpawned = true;
    }

    public void PlayerJoined(PlayerRef player)
    {
      if (!HasStateAuthority) return;

      Vector3 spawnPosition = Vector3.up * 2f + Vector3.right * (player.PlayerId * 3f);

      var playerObject = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
      var networkedPlayer = playerObject.GetComponentInChildren<NetworkedPlayer>();
      if (networkedPlayer == null)
      {
        Debug.LogError("NetworkedPlayer component not found in player prefab.");
        return;
      }

      Players.Add(player, networkedPlayer);
    }

    public void PlayerLeft(PlayerRef player)
    {
      if (!HasStateAuthority) return;
      if (!Players.TryGet(player, out var playerObject)) return;

      Debug.Log($"Player {player} left, despawning");
      Runner.Despawn(playerObject.Object);
      Players.Remove(player);
    }
  }
}