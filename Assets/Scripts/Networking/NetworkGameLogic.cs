using Fusion;
using UnityEngine;

namespace Networking
{
  public class NetworkGameLogic : NetworkBehaviour, IPlayerJoined, IPlayerLeft
  {
    [Header("Prefabs")]
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Follow Player")]
    [SerializeField]
    private FollowPlayer followPlayer;

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

      Vector3 baseSpawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
      Vector3 lateralOffset = spawnPoint != null ? spawnPoint.right : Vector3.right;
      Vector3 spawnPosition = baseSpawnPosition + lateralOffset * (player.PlayerId * 3f);

      if (Physics.Raycast(spawnPosition + Vector3.up * 5f, Vector3.down, out var groundHit, 10f,
        Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
      {
        spawnPosition = groundHit.point;
      }

      var playerObject = Runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);
      var networkedPlayer = playerObject.GetComponentInChildren<NetworkedPlayer>();
      playerObject.transform.SetParent(spawnPoint);
      if (networkedPlayer == null)
      {
        Debug.LogError("NetworkedPlayer component not found in player prefab.");
        return;
      }

      // Keep the player tracked
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