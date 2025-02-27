using Fusion;
using UnityEngine;

namespace Networking {
  public class NetworkGameLogic : NetworkBehaviour, IPlayerJoined, IPlayerLeft {
    [Header("Prefabs")]
    [SerializeField] private NetworkPrefabRef playerPrefab;  // Prefab del PlayerMaker (con NetworkedPlayer)
    [SerializeField] private NetworkPrefabRef avatarPrefab;  // Prefab del avatar (con AvatarMp)

    [Header("Follow Player")]
    [SerializeField] private FollowPlayer followPlayer;

    [Networked, Capacity(12)]
    public NetworkDictionary<PlayerRef, NetworkedPlayer> Players => default;

    public bool isSpawned;

    public override void Spawned() {
      base.Spawned();
      isSpawned = true;
    }

    public void PlayerJoined(PlayerRef player) {
      if (!HasStateAuthority) return;

      // Spawn del PlayerMaker (objeto que contiene el script NetworkedPlayer)
      var playerObject = Runner.Spawn(playerPrefab, Vector3.up, Quaternion.identity, player);
      var networkedPlayer = playerObject.GetComponentInChildren<NetworkedPlayer>();
      if (networkedPlayer == null) {
        Debug.LogError("No se encontró el componente NetworkedPlayer en el prefab del PlayerMaker.");
        return;
      }

      // Spawn del avatar a partir del prefab y configurarlo como hijo del PlayerMaker
      var avatarObject = Runner.Spawn(avatarPrefab, playerObject.transform.position, playerObject.transform.rotation, player);
      avatarObject.transform.SetParent(playerObject.transform);

      // Obtener el componente AvatarMp para configurar los targets de IK (manos y cabeza)
      var avatarMP = avatarObject.GetComponent<AvatarMp>();
      if (avatarMP != null) {
        if (networkedPlayer.leftController != null)
          avatarMP.SetLeftHandTarget(networkedPlayer.leftController.transform);
        else
          Debug.LogWarning("El PlayerMaker no tiene asignado leftController.");

        if (networkedPlayer.rightController != null)
          avatarMP.SetRightHandTarget(networkedPlayer.rightController.transform);
        else
          Debug.LogWarning("El PlayerMaker no tiene asignado rightController.");

        if (networkedPlayer.gaze != null)
          avatarMP.SetHeadTarget(networkedPlayer.gaze.transform);
        else
          Debug.LogWarning("El PlayerMaker no tiene asignado gaze.");
      } else {
        Debug.LogError("No se encontró el componente AvatarMp en el prefab del avatar.");
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
