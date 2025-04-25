using UnityEngine;

public class StartEvaluation : MonoBehaviour {
    [SerializeField] private Evaluator evaluator;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Trigger con " + other.gameObject.name + " " + other.gameObject.tag);
        if (hasTriggered) return;

        if (other.CompareTag("MainCamera")) {
            if (evaluator != null) {
                evaluator.gameObject.SetActive(true);
                hasTriggered = true;
            } else {
                Debug.LogWarning("Evaluator no asignado en StartEvaluation.");
            }
        }
    }
}

