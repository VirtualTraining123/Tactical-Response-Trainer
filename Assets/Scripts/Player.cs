using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] float health;
    [SerializeField] Transform head;
    public CameraShake cameraShake;

    private void Start()
    {
        BluetoothService.StartBluetoothConnection("ESP32_BT");
    }

    public void TakeDamage(float damage)
    {
        StartCoroutine(cameraShake.Shake());
        health -= damage;
        Debug.LogWarning(string.Format("Player health: {0}", health));
        //BluetoothService.WritetoBluetooth("F");
        // Verificar si la salud llega a cero o menos
        if (health <= 0)
        {

            // ir a la escena de Game Over pero a los 5 segundos
            Invoke("GameOver", 5f);
           
            


        }
    }

    private void GameOver()
    {
        // Obtener el nombre de la escena actual
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Ir a la escena de Game Over
        SceneManager.LoadScene("3 GameOver");
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
