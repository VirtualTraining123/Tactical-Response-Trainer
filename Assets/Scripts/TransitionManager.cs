using System.Collections;
using UnityEngine;

public class TransitionManager : MonoBehaviour {
    [Header("Referencia al efecto Blink")]
    [SerializeField] private FadeScreen fadeScreen;

    [Header("Configuración de tiempos (en segundos)")]
    [SerializeField] private float fadeInDuration = 3f;
    [SerializeField] private float darkDuration = 1f;  
    [SerializeField] private float fadeOutDuration = 3f;
    
    [Header("Escenarios")]
    [SerializeField] private GameObject externalScenarioPrefab; 
    [SerializeField] private GameObject internalScenarioPrefab; 
    
    [Header("Player Movement")]
    [SerializeField] private MonoBehaviour playerMovement; 

    private void Awake() {
        if (fadeScreen == null) {
            Debug.LogError("No se asignó el FadeScreen en TransitionManager.");
        }
    }

    public void StartTransition() {
        StartCoroutine(BlinkTransitionCoroutine());
    }

    private IEnumerator BlinkTransitionCoroutine() {
        // Establecemos la duración del fade dinámicamente
        fadeScreen.fadeDuration = fadeInDuration;
        fadeScreen.Fade(0, 1); // Fade In (oscurecer)
        yield return new WaitForSeconds(fadeInDuration);

        // Oscuridad total
        yield return new WaitForSeconds(darkDuration);

        // Transición de escenarios
        if (externalScenarioPrefab is not null)
            externalScenarioPrefab.SetActive(false);

        if (internalScenarioPrefab is not null)
            internalScenarioPrefab.SetActive(true);

        // Bloquear movimiento del jugador
        if (playerMovement is not null)
            playerMovement.enabled = false;

        // Fade Out (volver a la visibilidad)
        fadeScreen.fadeDuration = fadeOutDuration;
        fadeScreen.Fade(1, 0);
        yield return new WaitForSeconds(fadeOutDuration);
    }
}