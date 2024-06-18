using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatetocamera : MonoBehaviour
{
     public GameObject label;

    [Header("Player Camera")]
    public Transform playerCamera; // Assign the player's camera in the inspector
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
