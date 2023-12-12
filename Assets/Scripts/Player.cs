using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] Transform head;
    public ConnectionV2 vest;
    public void TakeDamage(float damage)
    {
        health -= damage;
        vest.Send("F"); //send value
        Debug.LogError(string.Format("Player health: {0}", health));

        // Verificar si la salud llega a cero o menos
        if (health <= 0)
        {
            // Reiniciar la escena
            RestartScene();
        }
    }

    public Vector3 GetHeadPosition()
    {
        return head.position;
    }

    private void RestartScene()
    {
        // Obtener el nombre de la escena actual
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Reiniciar la escena actual
        SceneManager.LoadScene(currentSceneName);
    }
}
