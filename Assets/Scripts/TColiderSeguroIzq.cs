using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TColiderSeguroIzq : MonoBehaviour
{
     
  [SerializeField] private SafetyColor SafetyColor;
  [SerializeField] private Pistol1 Pistol1;
    
 private void OnTriggerEnter(Collider collision)
    {
        // Verificar si el objeto que colisiona es una bala o tiene una etiqueta específica
        if (collision.gameObject.CompareTag("RightHand"))
        {
            Debug.LogAssertion("Colision con la mano derecha");
            Pistol1.callToggleSafety();
            SafetyColor.ToggleMaterial();
        }

    }
}
