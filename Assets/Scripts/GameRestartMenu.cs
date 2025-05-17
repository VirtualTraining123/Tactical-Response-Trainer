using AI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;
using Results;
using Scenes;
using System.Linq;


public class GameRestartMenu : MonoBehaviour {
  public ResultManagerType resultManagerType;
  [Header("Skybox")]
  public Material skyboxWin;
  public Material skyboxLose;
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
  public TMP_Text title;
  [Header("Enemy Display")]
  public GameObject enemyDisplayRoot;
  public Vector3 layoutDirection;
  public KillDisplayDummy enemyDisplayPrefab;

  private IResultManager resultManager;

  [Header("Player Camera")] public Transform playerCamera;

  private void Awake() {
    resultManager = ResultManagerFactory.Create(resultManagerType);
    EnableGameOverMenu();
    // Hook events
    restartButton.onClick.AddListener(RestartGame);
    quitButton.onClick.AddListener(QuitGame);
    DisplayCollectedData();
  }

  private void Update() {
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
    SceneTransitionManager.Singleton.GoToSceneAsync(Scene.Evaluation);
  }

  private void HideAll() {
    gameOverMenu.SetActive(false);
  }

  private void EnableGameOverMenu() {
    gameOverMenu.SetActive(true);
  }

  private async void DisplayCollectedData() {
    var result = await resultManager.LoadResult();
    remainingEnemies.text = $"Enemigos faltantes: {result.MissingEnemies}";
    injuredCivilians.text = $"Civiles heridos: {result.InjuredCivilians}";
    extraBulletsUsed.text = $"Balas extra usadas: {result.ExtraBulletsUsed}";
    timeOnScene.text = $"Tiempo en escena: {result.Time.ToString(CultureInfo.InvariantCulture)}s";
    agentDeath.text = $"Muerte del agente: {I18N.I18N.Get(result.AgentDeath)}";
    safetyActive.text = $"Seguridad activada: {result.SafetyOff}";
    finalScore.text = $"Puntaje final: {result.FinalScore}";
    title.text = result.Passed ? I18N.I18N.GetPassedText() : I18N.I18N.GetFailedText();
    RenderSettings.skybox = result.Passed ? skyboxWin : skyboxLose;

    var itemWidth = layoutDirection.normalized * 1;
    var fullWidth = result.Hits.Count * itemWidth;

    _ = result.Hits.Select((enemy, index) => {
      var enemyDisplay = Instantiate(
        enemyDisplayPrefab,
        enemyDisplayRoot.transform.position - fullWidth / 2 + itemWidth * index,
        enemyDisplayRoot.transform.rotation,
        enemyDisplayRoot.transform
      );
      enemyDisplay.DisplayDamagedPart(enemy.Value);
      return enemyDisplay;
    }).All(x=>true);
  }
}
