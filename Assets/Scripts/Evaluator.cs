using System.Linq;
using Results;
using Spawner;
using UnityEngine;

public class Evaluator : MonoBehaviour {
  [Header("Settings")] public long maxSimulationTime = 3;
  [SerializeField] public float maxScore = 10f;
  [SerializeField] public float minPassingScore = 6f;
  [SerializeField] public float scorePenaltyForEnemy = 1f;
  [SerializeField] public float scorePenaltyForCivilian = 2f;
  [SerializeField] public float scorePenaltyForExtraBullet = 0.25f;
  [SerializeField] public float scorePenaltyForLeavingSafetyOff = 1f;
  [SerializeField] public Pistol[] pistols;
  [SerializeField] public ResultManagerType resultManagerType;

  private IResultManager resultManager;
  private long simulationStartTime;
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


  private void Awake() {
    spawnManager = FindObjectOfType<SpawnManager>();
    parBulletCount = spawnManager.GetTotalEnemies();
    simulationStartTime = GetTime();
    totalEnemyCount = 0;
    enemiesKilled = 0;
    usedBulletCount = 0;
    civiliansKilled = 0;
    resultManager = ResultManagerFactory.Create(resultManagerType);
    resultManager.Clear();
  }


  private void FixedUpdate() {
    if (hasSimulationEnded) return;
    if (GetTime() < simulationStartTime + maxSimulationTime * 1000) return;
    EndSimulation();
  }

  private static long GetTime() => System.DateTimeOffset.Now.ToUnixTimeMilliseconds();
  public void OnEnemyKilled() => enemiesKilled++;
  public void OnCivilianKilled() => civiliansKilled++;
  public void OnBulletUsed() => usedBulletCount++;
  private float CheckSafetyPenalty() => pistols.Count(pistol => pistol.isSafetyOn) * scorePenaltyForLeavingSafetyOff;
  private int GetRemainingEnemies() => Mathf.Max(totalEnemyCount - enemiesKilled, 0);
  private float ConsiderMissedEnemies() => scorePenaltyForEnemy * GetRemainingEnemies();
  private float ConsiderKilledCivilians() => scorePenaltyForCivilian * civiliansKilled;
  private bool HasPassed(float score) => score < minPassingScore && !isPlayerDead;
  public float GetElapsedTime() => (GetTime() - simulationStartTime) / 1000f;
  public int GetCiviliansKilled() => civiliansKilled;
  public int GetEnemiesKilled() => enemiesKilled;
  public int GetExtraBulletsUsed() => Mathf.Max(usedBulletCount - parBulletCount, 0);
  private float ConsiderUsedBullets() => GetExtraBulletsUsed() * scorePenaltyForExtraBullet;

  public void OnReceiveShot() {
    isPlayerDead = true;
    EndSimulation();
  }

  public void EndSimulation() {
    hasSimulationEnded = true;
    totalEnemyCount = spawnManager.GetTotalEnemies();
    var score = maxScore;
    score -= ConsiderKilledCivilians();
    score -= ConsiderMissedEnemies();
    score -= CheckSafetyPenalty();
    score -= ConsiderUsedBullets();

    var evaluationResult = new EvaluationResult(
      GetElapsedTime(),
      civiliansKilled,
      totalEnemyCount - enemiesKilled,
      GetExtraBulletsUsed(),
      score,
      isPlayerDead,
      pistols.Count(pistol => pistol.isSafetyOn)
    );
    Debug.Log(evaluationResult.ToString());
    resultManager.SaveResult(evaluationResult);
    SceneTransitionManager.Singleton.GoToSceneAsync(HasPassed(score) ? FAILED_SCENE : PASSED_SCENE);
  }
}