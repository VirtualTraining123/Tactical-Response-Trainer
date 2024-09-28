using UnityEngine;

public class SceneFinish : MonoBehaviour
{
    private Evaluator evaluator;
    private bool isSimulationEnding; // Flag para verificar si la finalización de la simulación ya ha sido iniciada

    protected void Start()
    {
        evaluator = FindObjectOfType<Evaluator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto que colisiona tiene la etiqueta específica
        if (collision.gameObject.CompareTag("Button") && !isSimulationEnding)
        {
            Debug.Log("Botón pulsado");
            isSimulationEnding = true; // Marcar que la finalización de la simulación ha sido iniciada
            evaluator.EndSimulation();
        }
    }
}
