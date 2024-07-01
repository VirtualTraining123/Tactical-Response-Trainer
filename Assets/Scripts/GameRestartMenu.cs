using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;

public class GameRestartMenu : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject gameOverMenu;

    [Header("GameOver Menu Buttons")]
    public Button restartButton;
    public Button quitButton;

    public List<Button> returnButtons;

    [Header("Data Display")]
    public TMP_Text Enemigos_Faltantes;
    public TMP_Text Civiles_Heridos;
    public TMP_Text Cartuchos_extra_gastados;
    public TMP_Text Tiempo;
    public TMP_Text Muerte_de_agente;
    public TMP_Text Puntaje_Final;
    public TMP_Text Seguro_Colocado;

    [Header("Player Camera")]
    public Transform playerCamera; // Assign the player's camera in the inspector

    public float distanceFromPlayer = 2f; // Distance from the player to display the menu

    void Start()
    {
        EnableGameOverMenu();

        // Hook events
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);

        DisplayCollectedData();
    }

    void Update()
    {
        // Position the menu in front of the player
        if (playerCamera != null)
        {
            Vector3 directionToMenu = playerCamera.forward;
            Vector3 menuPosition = playerCamera.position + directionToMenu * distanceFromPlayer;

            // Position the menu
            //gameOverMenu.transform.position = menuPosition;

            // Rotate the menu to face the player
            gameOverMenu.transform.LookAt(playerCamera);
            gameOverMenu.transform.Rotate(0f, 180f, 0f); // Adjust the rotation to face the player correctly
        }
    }

    public void QuitGame()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    public void RestartGame()
    {
        PlayerPrefs.DeleteAll();

        HideAll();
       
       // ClearPlayerPrefsData(); // Limpia los datos almacenados en PlayerPrefs
       //espera 2 segundos antes de reiniciar el juego
       
        StartCoroutine(RestartGameRoutine());
       
    }

    IEnumerator RestartGameRoutine()
    {
        yield return new WaitForSeconds(5f);
        SceneTransitionManager.singleton.GoToScene(2);
    }

    public void HideAll()
    {
        gameOverMenu.SetActive(false);


    }

    public void EnableGameOverMenu()
    {
        gameOverMenu.SetActive(true);
    }

    private void DisplayCollectedData()
    {
        // Retrieve collected data from PlayerPrefs or wherever it's stored
        float time = PlayerPrefs.GetFloat("Tiempo", 0f);
        int enemiesNotEliminated = PlayerPrefs.GetInt("Enemigos_Faltantes", 0);
        int civiliansEliminated = PlayerPrefs.GetInt("Civiles_Heridos", 0);
        int ExtraShotsFired = PlayerPrefs.GetInt("Cartuchos_extra_gastados", 0);
        String agente = PlayerPrefs.GetString("Muerte_de_agente", "No");
        float finalScore = PlayerPrefs.GetFloat("Puntaje_Final", 0f);
        String Seguro = PlayerPrefs.GetString("Seguro", "No");

        // Update UI elements with collected data
        Enemigos_Faltantes.text ="Enemigos Faltanes: " + enemiesNotEliminated;
        Civiles_Heridos.text = "Civiles Heridos: " + civiliansEliminated;
        Tiempo.text = "Tiempo en escena: " + time.ToString() + "s";
        Cartuchos_extra_gastados.text = "Cartuchos extra gastados: " + ExtraShotsFired;
        Muerte_de_agente.text = "Muerte de agente: " + agente;
        Seguro_Colocado.text = "Seguro Colocado: " + Seguro;
        Puntaje_Final.text = "Puntaje Final: " + finalScore;
    }

    public void ClearPlayerPrefsData()
    {
        PlayerPrefs.DeleteKey("Enemy_Dead_Count");
        PlayerPrefs.DeleteKey("Civil_Dead_Count");
        PlayerPrefs.DeleteKey("Shots_Fired_Pistol");
        PlayerPrefs.DeleteKey("Elapsed_Time");
        PlayerPrefs.Save(); // Ensure changes are saved immediately
    }
}
