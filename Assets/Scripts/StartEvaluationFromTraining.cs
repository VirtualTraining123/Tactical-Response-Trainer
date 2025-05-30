using UnityEngine;

public class StartEvaluationFromTraining : MonoBehaviour {
  [SerializeField] private Evaluator evaluator;
  [SerializeField] private TransitionManager transitionManager;

  private void OnTriggerEnter(Collider other) {
    Debug.Log("Trigger con " + other.gameObject.name + " " + other.gameObject.tag);
    if (other.gameObject.CompareTag("MainCamera")) {
      // Se activa el evaluador si es necesario
      if (evaluator != null) {
        evaluator.enabled = true;
      }
      // Se inicia la transición blink
      if (transitionManager != null) {
        transitionManager.StartTransition();
      }
    }
  }
}
