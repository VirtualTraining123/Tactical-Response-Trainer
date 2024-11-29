using System;
using UnityEngine;


public class StartEvaluationFromTraining : MonoBehaviour {
  [SerializeField] private Evaluator evaluator;
    
  private void OnTriggerEnter(Collider trigger) {
    Debug.Log("Trigger with " + trigger.gameObject.name + trigger.gameObject.tag);
    if (trigger.gameObject.CompareTag("MainCamera")) {
      evaluator.enabled = true;
    }
  }
}