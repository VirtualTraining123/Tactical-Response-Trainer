using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Serialization;

public class GameRestartMenu : MonoBehaviour
{
    [Header("UI Pages")] public GameObject gameOverMenu;

    [Header("GameOver Menu Buttons")] public Button restartButton;
    public Button quitButton;

    public List<Button> returnButtons;

    [FormerlySerializedAs("Enemigos_Faltantes")] [Header("Data Display")]
    public TMP_Text enemigosFaltantes;

    [FormerlySerializedAs("Civiles_Heridos")]
    public TMP_Text civilesHeridos;

    [FormerlySerializedAs("Cartuchos_extra_gastados")]
    public TMP_Text cartuchosExtraGastados;

    [FormerlySerializedAs("Tiempo")] public TMP_Text tiempo;

    [FormerlySerializedAs("Muerte_de_agente")]
    public TMP_Text muerteDeAgente;

    [FormerlySerializedAs("Puntaje_Final")]
    public TMP_Text puntajeFinal;

    [FormerlySerializedAs("Seguro_Colocado")]
    public TMP_Text seguroColocado;

    [Header("Player Camera")] public Transform playerCamera; // Assign the player's camera in the inspector

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
            // Position the menu
            //gameOverMenu.transform.position = menuPosition;

            // Rotate the menu to face the player
            gameOverMenu.transform.LookAt(playerCamera);
            gameOverMenu.transform.Rotate(0f, 180f, 0f); // Adjust the rotation to face the player correctly
        }
    }

    private void QuitGame()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    private void RestartGame()
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
        SceneTransitionManager.Singleton.GoToScene(2);
    }

    private void HideAll()
    {
        gameOverMenu.SetActive(false);
    }

    private void EnableGameOverMenu()
    {
        gameOverMenu.SetActive(true);
    }

    private void DisplayCollectedData()
    {
        // Retrieve collected data from PlayerPrefs or wherever it's stored
        float time = PlayerPrefs.GetFloat("Tiempo", 0f);
        int enemiesNotEliminated = PlayerPrefs.GetInt("Enemigos_Faltantes", 0);
        int civiliansEliminated = PlayerPrefs.GetInt("Civiles_Heridos", 0);
        int extraShotsFired = PlayerPrefs.GetInt("Cartuchos_extra_gastados", 0);
        String agente = PlayerPrefs.GetString("Muerte_de_agente", "No");
        float finalScore = PlayerPrefs.GetFloat("Puntaje_Final", 0f);
        String seguro = PlayerPrefs.GetString("Seguro", "No");

        // Update UI elements with collected data
        enemigosFaltantes.text = "Enemigos Faltanes: " + enemiesNotEliminated;
        civilesHeridos.text = "Civiles Heridos: " + civiliansEliminated;
        tiempo.text = "Tiempo en escena: " + time + "s";
        cartuchosExtraGastados.text = "Cartuchos extra gastados: " + extraShotsFired;
        muerteDeAgente.text = "Muerte de agente: " + agente;
        seguroColocado.text = "Seguro Colocado: " + seguro;
        puntajeFinal.text = "Puntaje Final: " + finalScore;
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