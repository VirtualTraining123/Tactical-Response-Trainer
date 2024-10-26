using System.Threading.Tasks;
using UnityEngine;

namespace Results {
  public class ResultManager : IResultManager {
    public void SaveResult(EvaluationResult result) {
      PlayerPrefs.SetFloat(nameof(result.Time), result.Time);
      PlayerPrefs.SetInt(nameof(result.InjuredCivilians), result.InjuredCivilians);
      PlayerPrefs.SetInt(nameof(result.MissingEnemies), result.MissingEnemies);
      PlayerPrefs.SetInt(nameof(result.ExtraBulletsUsed), result.ExtraBulletsUsed);
      PlayerPrefs.SetFloat(nameof(result.FinalScore), result.FinalScore);
      PlayerPrefs.SetString(nameof(result.AgentDeath), result.AgentDeath.ToString());
      PlayerPrefs.SetString(nameof(result.SafetyOff), result.SafetyOff.ToString());
    }

    public Task<EvaluationResult> LoadResult() {
      var result = new EvaluationResult(
        PlayerPrefs.GetFloat(nameof(EvaluationResult.Time)),
        PlayerPrefs.GetInt(nameof(EvaluationResult.InjuredCivilians)),
        PlayerPrefs.GetInt(nameof(EvaluationResult.MissingEnemies)),
        PlayerPrefs.GetInt(nameof(EvaluationResult.ExtraBulletsUsed)),
        PlayerPrefs.GetFloat(nameof(EvaluationResult.FinalScore)),
        bool.Parse(PlayerPrefs.GetString(nameof(EvaluationResult.AgentDeath))),
        int.Parse(PlayerPrefs.GetString(nameof(EvaluationResult.SafetyOff)))
      );

      return Task.FromResult(result);
    }

    public void Clear() {
      PlayerPrefs.DeleteAll();
    }
  }
}