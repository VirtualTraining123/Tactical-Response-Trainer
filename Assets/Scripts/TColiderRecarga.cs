using UnityEngine;
using UnityEngine.Serialization;

// ReSharper disable once InconsistentNaming
public class TColiderRecarga : MonoBehaviour
{
   
      [FormerlySerializedAs("Pistol1")] [SerializeField] private Pistol1 pistol1;
    
 private void OnTriggerEnter(Collider collision)
    {
        // Verificar si el objeto que colisiona es una bala o tiene una etiqueta específica
        if (collision.gameObject.CompareTag("Magazine"))
        {
            Debug.Log("Colision con la mano izquierda");
            pistol1.CallReload();
            //destruimos el objeto Magazine
            Destroy(collision.gameObject);

        }

    }
}