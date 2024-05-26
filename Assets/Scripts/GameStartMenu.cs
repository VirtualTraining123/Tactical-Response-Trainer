using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartMenu : MonoBehaviour
{
    [Header("UI Pages")]
    public GameObject mainMenu;
    public GameObject options;
    public GameObject about;
    
    [Header("Main Menu Buttons")]
    public Button startButton;
    public Button optionButton;
    public Button aboutButton;
    public Button quitButton;

    public List<Button> returnButtons;

    [Header("Player Camera")]
    public Transform playerCamera; // Assign the player's camera in the inspector

    public float distanceFromPlayer = 2f; // Distance from the player to display the menu

    // Start is called before the first frame update
    void Start()
    {
        EnableMainMenu();

        //Hook events
        startButton.onClick.AddListener(StartGame);
        optionButton.onClick.AddListener(EnableOption);
        aboutButton.onClick.AddListener(EnableAbout);
        quitButton.onClick.AddListener(QuitGame);

        foreach (var item in returnButtons)
        {
            item.onClick.AddListener(EnableMainMenu);
        }
    }

    void Update()
    {
        // Position the menu in front of the player
        if (playerCamera != null)
        {
            Vector3 directionToMenu = playerCamera.forward;
            Vector3 menuPosition = playerCamera.position + directionToMenu * distanceFromPlayer;



            // Rotate the menu to face the player
            mainMenu.transform.LookAt(playerCamera);
            mainMenu.transform.Rotate(0f, 180f, 0f); // Adjust the rotation to face the player correctly
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        HideAll();
        ClearPlayerPrefsData();
        SceneTransitionManager.singleton.GoToSceneAsync(1);
    }

    public void HideAll()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(false);
    }

    public void EnableMainMenu()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);
        about.SetActive(false);
    }
    public void EnableOption()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
        about.SetActive(false);
    }
    public void EnableAbout()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        about.SetActive(true);
    }

     public void ClearPlayerPrefsData()
{
    PlayerPrefs.DeleteKey("EnemiesEliminated");
    PlayerPrefs.DeleteKey("CiviliansEliminated");
    PlayerPrefs.DeleteKey("ShotsFired");
    PlayerPrefs.DeleteKey("TimeElapsed");
}
}
