using UnityEngine;

public class SceneFinish : MonoBehaviour {
  private Evaluator evaluator;
  private bool isSimulationEnding;

  protected void Start() {
    evaluator = FindObjectOfType<Evaluator>();
  }

  private void OnCollisionEnter(Collision collision) {
    if (!collision.gameObject.CompareTag("Button") || isSimulationEnding) return;
    Debug.Log("End scene pressed");
    isSimulationEnding = true;
    evaluator.EndSimulation();
  }
}