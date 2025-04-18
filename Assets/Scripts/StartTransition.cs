using UnityEngine;

public class StartTransition : MonoBehaviour {
    [SerializeField] private TransitionManager transitionManager;
    [SerializeField] private GateOpener gateOpener;
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Trigger con " + other.gameObject.name + " " + other.gameObject.tag);
        if (hasTriggered) return;

        if (other.CompareTag("MainCamera")) {
          /*  if (gateOpener != null) {
                gateOpener.StartOpening();
            } else {
                Debug.LogWarning("GateOpener no asignado.");
            }*/

            if (transitionManager != null) {
                transitionManager.StartTransition();
                hasTriggered = true;
            } else {
                Debug.LogWarning("TransitionManager no asignado en StartTransition.");
            }
        }
    }
}

