using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TColiderSeguro : MonoBehaviour
{
     
  [SerializeField] private SafetyColor SafetyColor;
  [SerializeField] private Pistol1 Pistol1;
    
 private void OnTriggerEnter(Collider collision)
    {
        // Verificar si el objeto que colisiona es una bala o tiene una etiqueta específica
        if (collision.gameObject.CompareTag("LeftHand"))
        {
            Debug.LogAssertion("Colision con la mano izquierda");
            Pistol1.callToggleSafety();
            SafetyColor.ToggleMaterial();
        }

    }
}
