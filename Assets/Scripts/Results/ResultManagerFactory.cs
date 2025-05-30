using System;

namespace Results {
  public static class ResultManagerFactory {
    public static IResultManager Create(ResultManagerType type) {
      return type switch {
        ResultManagerType.PlayerPrefs => new PlayerPrefsResultManager(),
        ResultManagerType.FixedResultManager => new FixedResultManager(),
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };
    }
  }
}
