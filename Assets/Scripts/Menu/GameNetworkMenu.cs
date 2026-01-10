using System;
using Fusion;
using Networking;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu {
  public class GameNetworkMenu : MonoBehaviour {
    public bool autoHost = true;
    public bool autoJoin = false;
    public Button hostButton;
    public Button joinButton;
    
    public ConnectedPlayersDisplay connectedPlayersDisplay;
    public NetworkRunner runner;
    public InputManager inputManager;
    


    private void Start() {
      hostButton.onClick.AddListener(HostGame);
      joinButton.onClick.AddListener(JoinGame);
      if (! (autoHost ^ autoJoin)) {
        Debug.LogWarning("Both autoHost and autoJoin are set to true. Only one should be true at a time.");
      }
      if (autoHost) HostGame();
      else if (autoJoin) JoinGame();
    }

    private async void StartGame(GameMode mode) {
      hostButton.interactable = false;
      joinButton.interactable = false;
      try {
        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

        // Start or join (depends on gamemode) a session with a specific name
        var result = await runner.StartGame(
          new StartGameArgs {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
          }
        );
        Log(result);
        runner.AddGlobal(inputManager);
        runner.AddCallbacks(inputManager);
      } catch (Exception e) {
        Log("Exception: " + e.Message);
      }
    }

    private void Log(object str) {
      Debug.Log(str);
      connectedPlayersDisplay.Log(str.ToString());
    }

    private void HostGame() {
      Log("Host Game");
      StartGame(GameMode.Host);
    }

    private void JoinGame() {
      Log("Join Game");
      StartGame(GameMode.Client);
    }
  }
}