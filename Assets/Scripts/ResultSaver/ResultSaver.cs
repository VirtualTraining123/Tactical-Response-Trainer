using UnityEngine;

namespace ResultSaver {
  public abstract class ResultSaver: MonoBehaviour {
    public abstract void SaveResult(EvaluationResult result);
  }
}