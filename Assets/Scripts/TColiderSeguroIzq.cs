using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable once InconsistentNaming
public class TColiderSeguroIzq : MonoBehaviour
{
     
  [FormerlySerializedAs("SafetyColor")] [SerializeField] private SafetyColor safetyColor;
  [FormerlySerializedAs("Pistol1")] [SerializeField] private Pistol1 pistol1;
    
 private void OnTriggerEnter(Collider collision)
    {
        // Verificar si el objeto que colisiona es una bala o tiene una etiqueta específica
        if (collision.gameObject.CompareTag("RightHand"))
        {
            Debug.LogAssertion("Colision con la mano derecha");
            pistol1.CallToggleSafety();
            safetyColor.ToggleMaterial();
        }

    }
}
