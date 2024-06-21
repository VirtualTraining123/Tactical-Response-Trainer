using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneNext : MonoBehaviour
{
  //  private Evaluator evaluator;
    
protected void Start()
    {
        
        
        


    }
 private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto que colisiona es una bala o tiene una etiqueta específica
        if (collision.gameObject.CompareTag("Button"))
        {
            Debug.LogAssertion("Boton pulsado");
            SceneTransitionManager.singleton.GoToSceneAsync(2);
        }

    }
}