using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNext : MonoBehaviour
{
    private bool isSceneChanging = false; // Flag para verificar si el cambio de escena ya ha sido iniciado

    protected void Start()
    {
        // Inicializaciones necesarias
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto que colisiona tiene la etiqueta específica
        if (collision.gameObject.CompareTag("Button") && !isSceneChanging)
        {
            Debug.Log("Botón pulsado");
            isSceneChanging = true; // Marcar que el cambio de escena ha sido iniciado
            SceneTransitionManager.singleton.GoToSceneAsync(2);
        }
    }


}
