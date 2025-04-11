using Fusion;
using UnityEngine;

namespace Networking {
  public class NetworkGameLogic : NetworkBehaviour, IPlayerJoined, IPlayerLeft {
    [Header("Prefabs")] [SerializeField]
    private NetworkPrefabRef playerPrefab; // Prefab del PlayerMaker (con NetworkedPlayer)

    [Header("Follow Player")] [SerializeField]
    private FollowPlayer followPlayer;

    [Networked, Capacity(12)] public NetworkDictionary<PlayerRef, NetworkedPlayer> Players => default;

    public bool isSpawned;

    public override void Spawned() {
      base.Spawned();
      isSpawned = true;
    }

    public void PlayerJoined(PlayerRef player) {
      if (!HasStateAuthority) return;

      // Spawn del PlayerMaker (objeto que contiene el script NetworkedPlayer)
      var playerObject = Runner.Spawn(playerPrefab, Vector3.up * 5, Quaternion.identity, player);
      var networkedPlayer = playerObject.GetComponentInChildren<NetworkedPlayer>();
      if (networkedPlayer == null) {
        Debug.LogError("No se encontró el componente NetworkedPlayer en el prefab del PlayerMaker.");
        return;
      }

      // Asignar el player para que se siga (por ejemplo, en una cámara o UI de seguimiento)
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