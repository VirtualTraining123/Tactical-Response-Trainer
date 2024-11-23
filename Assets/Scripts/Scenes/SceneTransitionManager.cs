using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = Scenes.Scene;

public class SceneTransitionManager : MonoBehaviour {
  public FadeScreen fadeScreen;
  public static SceneTransitionManager Singleton;

  private void Awake() {
    if (Singleton && Singleton != this)
      Destroy(Singleton);

    Singleton = this;
  }

  public void GoToSceneAsync(Scene sceneIndex) {
    fadeScreen.FadeOut();
    var operation = SceneManager.LoadSceneAsync((int) sceneIndex);
    if (operation == null) return;
    operation.allowSceneActivation = true;
  }
}