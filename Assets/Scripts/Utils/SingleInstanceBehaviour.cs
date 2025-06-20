using JetBrains.Annotations;
using Spawner;
using UnityEngine;
namespace Utils {
  public abstract class Instance<T> : MonoBehaviour where T : MonoBehaviour {
    [CanBeNull]
    private static T _instance;
    public static T Get() {
      if (_instance) return _instance;
      return _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);
    }
  }
}
