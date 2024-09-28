using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour {
  public TextMeshProUGUI enemyRemaining; //no lo arreglo porque crashea rider
  public TextMeshProUGUI civiliansInjured;
  public TextMeshProUGUI safetyLock;
  public TextMeshProUGUI bullets;
  public TextMeshProUGUI timeText;
  private Evaluator evaluator;

  void Start() {
    evaluator = FindObjectOfType<Evaluator>();
  }

  void Update() {
    // Actualiza el texto con los valores actuales del evaluador
    enemyRemaining.text = "Enemigos Eliminados: " + evaluator.GetEnemiesKilled();
    civiliansInjured.text = "Civiles Heridos: " + evaluator.GetCiviliansKilled();
    timeText.text = "Tiempo: " + evaluator.GetElapsedTime().ToString("F2");
    safetyLock.text = "Seguro: " + "bla";
    bullets.text = "Balas en cargador: " + "TODO"; // TODO: FIXME
  }
}