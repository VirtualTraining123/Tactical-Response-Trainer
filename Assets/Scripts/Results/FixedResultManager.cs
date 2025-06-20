using AI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Results {
  public class FixedResultManager : IResultManager {
    public void SaveResult(EvaluationResult result) {
      // Empty
    }

    public Task<EvaluationResult> LoadResult() {
      var result = new EvaluationResult(
        69,
        5,
        5,
        2,
        99,
        true,
        0,
        new() {
          {
            "enemy1", new() {
              BodyPart.HEAD
            }
          }, {
            "enemy2", new() {
              BodyPart.TORSO_HIGH
            }
          },
        }
      );

      return Task.FromResult(result);
    }

    public void Clear() {
      // FIXME: Delete only the keys that are used to store the result
      PlayerPrefs.DeleteAll();
    }
  }
}
