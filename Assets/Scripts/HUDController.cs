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
    enemyRemaining.text = "Enemigos Eliminados: " + evaluator.GetEnemiesEliminated();
    civiliansInjured.text = "Civiles Heridos: " + evaluator.GetCiviliansInjured();
    timeText.text = "Tiempo: " + evaluator.GetElapsedTime().ToString("F2");
    safetyLock.text = "Seguro: " + evaluator.GetSafetyLock();
    bullets.text = "Balas en cargador: " + evaluator.GetBullets();
  }
}