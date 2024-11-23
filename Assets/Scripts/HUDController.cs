using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour {
  [CanBeNull] public TextMeshProUGUI enemyRemaining; //no lo arreglo porque crashea rider
  [CanBeNull] public TextMeshProUGUI civiliansInjured;
  [CanBeNull] public TextMeshProUGUI safetyLock;
  [CanBeNull] public TextMeshProUGUI bullets;
  [CanBeNull] public TextMeshProUGUI timeText;
  private Evaluator evaluator;

  void Start() {
    evaluator = FindObjectOfType<Evaluator>();
  }

  void Update() {
    // Actualiza el texto con los valores actuales del evaluador
    // enemyRemaining.text = "Enemigos Eliminados: " + evaluator.GetEnemiesKilled();
    // civiliansInjured.text = "Civiles Heridos: " + evaluator.GetCiviliansKilled();
    if (timeText) timeText.text = "Tiempo: " + evaluator.GetElapsedTime().ToString("F2");
    // safetyLock.text = "Seguro: " + "bla";
    // bullets.text = "Balas en cargador: " + "TODO"; // TODO: FIXME
  }
}