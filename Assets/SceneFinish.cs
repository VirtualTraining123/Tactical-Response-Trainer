using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneFinish : MonoBehaviour
{
    private Evaluator evaluator;

protected void Start()
    {
        
        
        evaluator = FindObjectOfType<Evaluator>();


    }
 private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto que colisiona es una bala o tiene una etiqueta específica
        if (collision.gameObject.CompareTag("Button"))
        {
            Debug.LogAssertion("Boton pulsado");
            evaluator.EarlyEndSimulation();
        }

    }
}