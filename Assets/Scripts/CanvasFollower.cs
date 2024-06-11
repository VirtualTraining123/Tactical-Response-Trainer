using UnityEngine;

public class CanvasFollower : MonoBehaviour
{
    public Transform player; // Asigna el transform del jugador en el inspector
    public Vector3 offset; // Usa este offset para ajustar la posición del canvas relativo al jugador
    private Vector3 initialPosition;

    void Start()
    {
        // Calcula la posición inicial del canvas basado en el offset
        initialPosition = player.position + player.forward * offset.z + player.up * offset.y + player.right * offset.x;
        transform.position = initialPosition;

        // Asegura que el canvas mire hacia el jugador y realiza una rotación de 180 grados una vez
        transform.LookAt(player);
        transform.Rotate(0, 180, 0); 
    }

    void Update()
    {
        // Posiciona el canvas frente al jugador cada frame
        Vector3 targetPosition = player.position + player.forward * offset.z + player.up * offset.y + player.right * offset.x;
        transform.position = targetPosition;

        // Asegura que el canvas siempre mire hacia el jugador, pero no acumule rotaciones
        transform.LookAt(player.position);
        transform.Rotate(0, 180, 0); // Rotar 180 grados para que el texto esté correctamente orientado
    }
}
