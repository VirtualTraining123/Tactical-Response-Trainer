using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using Results;
using Scenes;


public class GameRestartMenu : MonoBehaviour {
  [Header("UI Pages")] public GameObject gameOverMenu;

  [Header("GameOver Menu Buttons")] public Button restartButton;
  public Button quitButton;

  [Header("Data Display")] public TMP_Text remainingEnemies;

  public TMP_Text injuredCivilians;

  public TMP_Text extraBulletsUsed;

  public TMP_Text timeOnScene;

  public TMP_Text agentDeath;

  public TMP_Text safetyActive;

  public TMP_Text finalScore;

  [Header("Player Camera")] public Transform playerCamera;

  void Awake() {
    EnableGameOverMenu();

    // Hook events
    restartButton.onClick.AddListener(RestartGame);
    quitButton.onClick.AddListener(QuitGame);

    DisplayCollectedData();
  }

  void Update() {
    if (!playerCamera) return;
    gameOverMenu.transform.LookAt(playerCamera);
    gameOverMenu.transform.Rotate(0f, 180f, 0f);
  }

  private static void QuitGame() {
    PlayerPrefs.DeleteAll();
    Application.Quit();
  }

  private void RestartGame() {
    HideAll();
    SceneTransitionManager.Singleton.GoToSceneAsync((int) Scene.Evaluation);
  }

  private void HideAll() {
    gameOverMenu.SetActive(false);
  }

  private void EnableGameOverMenu() {
    gameOverMenu.SetActive(true);
  }

  private async void DisplayCollectedData() {
    var result = await ResultManagerFactory.Create().LoadResult();
    Debug.Log(result.ToString());
    remainingEnemies.text = $"Enemigos faltantes: {result.MissingEnemies}";
    injuredCivilians.text = $"Civiles heridos: {result.InjuredCivilians}";
    extraBulletsUsed.text = $"Balas extra usadas: {result.ExtraBulletsUsed}";
    timeOnScene.text = $"Tiempo en escena: {result.Time.ToString(CultureInfo.InvariantCulture)}s";
    agentDeath.text = $"Muerte del agente: {result.AgentDeath}";
    safetyActive.text = $"Seguridad activada: {result.SafetyOff}";
    finalScore.text = $"Puntaje final: {result.FinalScore}";
  }
}