using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class ColiderSeguro : MonoBehaviour
{
    [FormerlySerializedAs("SafetyColor")] [SerializeField] private SafetyColor safetyColor;
    [FormerlySerializedAs("Pistol")] [SerializeField] private Pistol pistol;

    private bool isSafetyCooldown; // Nueva variable para el enfriamiento del seguro
    private readonly float safetyCooldownDuration = 0.5f; // Duración del enfriamiento del seguro

    private void OnTriggerEnter(Collider collision)
    {
        // Verificar si el objeto que colisiona es una bala o tiene una etiqueta específica
        if (collision.gameObject.CompareTag("LeftHand"))
        {
            Debug.LogAssertion("Colision con la mano izquierda");
            if (!isSafetyCooldown)
            {
                pistol.CallToggleSafety();
                safetyColor.ToggleMaterial();
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
