using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColiderSeguro : MonoBehaviour
{
     
  [SerializeField] private SafetyColor SafetyColor;
  [SerializeField] private Pistol Pistol;
    
 private void OnTriggerEnter(Collider collision)
    {
        // Verificar si el objeto que colisiona es una bala o tiene una etiqueta específica
        if (collision.gameObject.CompareTag("LeftHand"))
        {
            Debug.LogAssertion("Colision con la mano izquierda");
            Pistol.callToggleSafety();
            SafetyColor.ToggleMaterial();
        }

    }
}
