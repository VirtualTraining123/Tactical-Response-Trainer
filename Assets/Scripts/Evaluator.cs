using System.Linq;
using ResultSaver;
using Spawner;
using UnityEngine;

public class Evaluator : MonoBehaviour {
  [Header("Settings")] public float maxSimulationTime = 90f;
  [SerializeField] public float maxScore = 10f;
  [SerializeField] public float minPassingScore = 6f;
  [SerializeField] public float scorePenaltyForEnemy = 1f;
  [SerializeField] public float scorePenaltyForCivilian = 2f;
  [SerializeField] public float scorePenaltyForExtraBullet = 0.25f;
  [SerializeField] public float scorePenaltyForLeavingSafetyOff = 1f;
  [SerializeField] public Pistol[] pistols;
  [SerializeField] public ResultSaver.ResultSaver resultSaver;

  private float elapsedTime;
  private bool hasSimulationEnded;
  private bool isPlayerDead;
  private bool hasSafetyBeenLeftOn;

  private int enemiesKilled;
  private int civiliansKilled;
  private int totalEnemyCount;
  private int parBulletCount;
  private int usedBulletCount;
  private SpawnManager spawnManager;

  private const int PASSED_SCENE = 4;
  private const int FAILED_SCENE = 3;


  private void Start() {
    spawnManager = FindObjectOfType<SpawnManager>();
    parBulletCount = spawnManager.GetTotalEnemies();
    elapsedTime = 0f;
    totalEnemyCount = 0;
    enemiesKilled = 0;
    usedBulletCount = 0;
    civiliansKilled = 0;
  }

  private void Update() {
    if (hasSimulationEnded) return;

    elapsedTime += Time.deltaTime;
    if (elapsedTime >= maxSimulationTime) return;
    EndSimulation();
  }

  public void OnEnemyKilled() => enemiesKilled++;

  public void OnCivilianKilled() => civiliansKilled++;

  public void OnBulletUsed() => usedBulletCount++;

  private float CheckSafetyPenalty() => pistols.Count(pistol => pistol.isSafetyOn) * scorePenaltyForLeavingSafetyOff;

  private float ConsiderMissedEnemies() => scorePenaltyForEnemy * (totalEnemyCount - enemiesKilled);
  private float ConsiderKilledCivilians() => scorePenaltyForCivilian * civiliansKilled;

  private bool HasPassed(float score) => score < minPassingScore && !isPlayerDead;

  public float GetElapsedTime() => elapsedTime;

  public int GetCiviliansKilled() => civiliansKilled;

  public int GetEnemiesKilled() => enemiesKilled;

  private float ConsiderUsedBullets() {
    var total = usedBulletCount - parBulletCount;
    return total > 0 ? scorePenaltyForExtraBullet * total : 0f;
  }

  public void OnReceiveShot() {
    isPlayerDead = true;
    EndSimulation();
  }

  public void EndSimulation() {
    totalEnemyCount = spawnManager.GetTotalEnemies();
    var score = maxScore;
    score -= ConsiderKilledCivilians();
    score -= ConsiderMissedEnemies();
    score -= CheckSafetyPenalty();
    score -= ConsiderUsedBullets();

    hasSimulationEnded = true;
    var evaluationResult = new EvaluationResult(
      elapsedTime,
      civiliansKilled,
      totalEnemyCount - enemiesKilled,
      usedBulletCount - parBulletCount,
      score,
      isPlayerDead,
      pistols.Count(pistol => pistol.isSafetyOn)
    );
    resultSaver.SaveResult(evaluationResult);
    SceneTransitionManager.Singleton.GoToSceneAsync(HasPassed(score) ? FAILED_SCENE : PASSED_SCENE);
  }
}