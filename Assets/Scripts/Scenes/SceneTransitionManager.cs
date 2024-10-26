using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour {
  public FadeScreen fadeScreen;
  public static SceneTransitionManager Singleton;

  private void Awake() {
    if (Singleton && Singleton != this)
      Destroy(Singleton);

    Singleton = this;
  }

  public void GoToSceneAsync(int sceneIndex) {
    fadeScreen.FadeOut();
    var operation = SceneManager.LoadSceneAsync(sceneIndex);
    if (operation == null) return;
    operation.allowSceneActivation = true;
  }
}