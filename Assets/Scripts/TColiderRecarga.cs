using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TColiderRecarga : MonoBehaviour
{
   
      [SerializeField] private Pistol1 Pistol1;
    
 private void OnTriggerEnter(Collider collision)
    {
        // Verificar si el objeto que colisiona es una bala o tiene una etiqueta específica
        if (collision.gameObject.CompareTag("Magazine"))
        {
            Debug.Log("Colision con la mano izquierda");
            Pistol1.callReload();
            //destruimos el objeto Magazine
            Destroy(collision.gameObject);

        }

    }
}