using UnityEngine;

public class Rotatetocamera : MonoBehaviour
{
     public GameObject label;

    [Header("Player Camera")]
    public Transform playerCamera; // Assign the player's camera in the inspector

      void Update()
    {
        // Position the menu in front of the player
        if (playerCamera != null)
        {
           
            // Rotate the menu to face the player
            label.transform.LookAt(playerCamera);
            label.transform.Rotate(0f, 0f, 0f); // Adjust the rotation to face the player correctly
        }
    }
}
