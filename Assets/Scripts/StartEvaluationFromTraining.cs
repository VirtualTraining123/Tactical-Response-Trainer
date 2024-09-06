using UnityEngine;
using UnityEngine.SceneManagement;

// Clase para iniciar la evaluación desde el entrenamiento

public class StartEvaluationFromTraining : MonoBehaviour
{
    // Variable para almacenar el nombre o índice de la siguiente escena
    [SerializeField] private string nextSceneName;

    [SerializeField] private BloodEffectPlane bloodEffectPlane;

    // Método llamado cuando otro collider entra en contacto con el collider de este objeto
    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto que colisiona es una bala o tiene una etiqueta específica
        if (collision.gameObject.CompareTag("Bullet"))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}