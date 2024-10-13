using System.Collections;
using UnityEngine;

public class BloodEffectPlane : MonoBehaviour {
  private Renderer rend;
  private Color startColor;
  private Coroutine currentCoroutine;
  [SerializeField] private float durationSeconds = 2.5f;

  private void Start() {
    rend = GetComponent<Renderer>();
    startColor = rend.material.color;
    rend.material.color = new Color(startColor.r, startColor.g, startColor.b, 0);
  }

  public void ShowBloodEffect() {
    if (currentCoroutine != null) {
      StopCoroutine(currentCoroutine);
    }
    currentCoroutine = StartCoroutine(ChangeTransparencyOverTime(startColor.a, 1, durationSeconds));
  }


  private IEnumerator ChangeTransparencyOverTime(float startAlpha, float targetAlpha, float parameterDuration) {
    var timer = 0f;
    var currentColor = rend.material.color;

    while (timer < parameterDuration) {
      var alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / parameterDuration);
      currentColor.a = alpha;
      rend.material.color = currentColor;
      timer += Time.deltaTime;
      yield return null;
    }

    // Ensure we end at the target alpha
    currentColor.a = targetAlpha;
    rend.material.color = currentColor;
  }
}