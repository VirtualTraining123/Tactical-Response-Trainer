using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFinish : MonoBehaviour
{
    private Evaluator evaluator;
    private bool isSimulationEnding = false; // Flag para verificar si la finalización de la simulación ya ha sido iniciada

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
            evaluator.EarlyEndSimulation();
        }
    }
}
