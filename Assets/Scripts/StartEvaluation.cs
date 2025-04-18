using UnityEngine;

public class StartEvaluation : MonoBehaviour {
    [SerializeField] private Evaluator evaluator;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other) {
        if (hasTriggered) return;

        if (other.CompareTag("MainCamera")) {
            if (evaluator != null) {
                evaluator.enabled = true;
                hasTriggered = true;
            } else {
                Debug.LogWarning("Evaluator no asignado en StartEvaluation.");
            }
        }
    }
}

