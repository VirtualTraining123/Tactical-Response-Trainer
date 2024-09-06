using UnityEngine;

public class SceneNext : MonoBehaviour
{
    private bool isSceneChanging; // Flag para verificar si el cambio de escena ya ha sido iniciado

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto que colisiona tiene la etiqueta específica
        if (collision.gameObject.CompareTag("Button") && !isSceneChanging)
        {
            Debug.Log("Botón pulsado");
            isSceneChanging = true; // Marcar que el cambio de escena ha sido iniciado
            SceneTransitionManager.Singleton.GoToSceneAsync(2);
        }
    }


}
