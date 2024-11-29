using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class WristData : MonoBehaviour {
  [CanBeNull] public TextMeshProUGUI timeText;
  [SerializeField] private Evaluator evaluator;

  private void Update() {
    var text = "";
    text += "Tiempo: " + evaluator.GetElapsedTime().ToString("F2") + "\n";
    text += "Evaluando? " + (evaluator.isActiveAndEnabled ? "Sí" : "No") + "\n";
    if (timeText) timeText.text = text;
  }
}