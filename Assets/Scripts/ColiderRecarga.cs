using UnityEngine;
using UnityEngine.Serialization;

public class ColiderRecarga : MonoBehaviour
{
   
      [FormerlySerializedAs("Pistol")] [SerializeField] private Pistol pistol;
    
 private void OnTriggerEnter(Collider collision)
    {
        // Verificar si el objeto que colisiona es una bala o tiene una etiqueta específica
        if (collision.gameObject.CompareTag("Magazine"))
        {
            Debug.Log("Colision con la mano izquierda");
            pistol.CallReload();
            //destruimos el objeto Magazine
            Destroy(collision.gameObject);

        }

    }
}