using UnityEngine;

namespace ResultSaver {
  public class PlayerPrefResultSaver : ResultSaver {
    public override void SaveResult(EvaluationResult result) {
      PlayerPrefs.SetFloat(nameof(result.Time), result.Time);
      PlayerPrefs.SetInt(nameof(result.InjuredCivilians), result.InjuredCivilians);
      PlayerPrefs.SetInt(nameof(result.MissingEnemies), result.MissingEnemies);
      PlayerPrefs.SetInt(nameof(result.ExtraBulletsUsed), result.ExtraBulletsUsed);
      PlayerPrefs.SetFloat(nameof(result.FinalScore), result.FinalScore);
      PlayerPrefs.SetString(nameof(result.AgentDeath), result.AgentDeath.ToString());
      PlayerPrefs.SetString(nameof(result.SafetyOff), result.SafetyOff.ToString());
    }
  }
}