using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    public TextMeshProUGUI Enemyremaining;
    public TextMeshProUGUI CiviliansInjured;
    public TextMeshProUGUI Safetylock;
    public TextMeshProUGUI bullets;
    public TextMeshProUGUI timeText;
    private Evaluator evaluator;

    void Start()
    {
        evaluator = FindObjectOfType<Evaluator>();
    }

    void Update()
    {
        // Actualiza el texto con los valores actuales del evaluador
        Enemyremaining.text = "Enemigos Eliminados: " + evaluator.GetEnemiesEliminated();
        CiviliansInjured.text = "Civiles Heridos: " + evaluator.GetCiviliansInjured();
        timeText.text = "Tiempo: " + evaluator.GetElapsedTime().ToString("F2");
        Safetylock.text = "Seguro: " + evaluator.GetSafetyLock();
        bullets.text = "Balas en cargador: " + evaluator.GetBullets();
    }
}
