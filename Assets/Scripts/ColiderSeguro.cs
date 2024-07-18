using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColiderSeguro : MonoBehaviour
{
    [SerializeField] private SafetyColor SafetyColor;
    [SerializeField] private Pistol Pistol;

    private bool isSafetyCooldown = false; // Nueva variable para el enfriamiento del seguro
    private float safetyCooldownDuration = 0.5f; // Duración del enfriamiento del seguro

    private void OnTriggerEnter(Collider collision)
    {
        // Verificar si el objeto que colisiona es una bala o tiene una etiqueta específica
        if (collision.gameObject.CompareTag("LeftHand"))
        {
            Debug.LogAssertion("Colision con la mano izquierda");
            if (!isSafetyCooldown)
            {
                Pistol.callToggleSafety();
                SafetyColor.ToggleMaterial();
                StartCoroutine(SafetyCooldown());
            }
        }
    }

    private IEnumerator SafetyCooldown()
    {
        isSafetyCooldown = true;
        yield return new WaitForSeconds(safetyCooldownDuration);
        isSafetyCooldown = false;
    }
}
