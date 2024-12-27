using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;

namespace Menu {
  public class ConnectedPlayersDisplay : MonoBehaviour {
    [SerializeField] private TMP_Text connectedPlayersText;
    [SerializeField] private NetworkGameLogic gameLogic;
    [SerializeField] private NetworkRunner networkRunner;
    private string _logData = "";

    public void Log(string str) {
      _logData += str + "\n";
    }

    public void Update() {
      var localPlayer = $"Local: {networkRunner.LocalPlayer}\n";
      var connectedPlayers = $"Connected Players: {networkRunner.ActivePlayers.Count()} ({(gameLogic.IsSpawned ? gameLogic.Players.Count : -1)})\n";
      var players = gameLogic.IsSpawned ? gameLogic.Players.Aggregate("", (acc, player) => $"{acc}- {player.Key}\n") : "Not spawned :c";
      connectedPlayersText.text = localPlayer + connectedPlayers + players + _logData;
    }
  }
}