using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyColor : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer; // El Renderer de la pieza que cambiará de material
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material safetyOnMaterial;

     [SerializeField] private bool isSafetyOn = false;

    public void ToggleMaterial()
    {
        if (targetRenderer == null)
        {
            Debug.LogWarning("Target Renderer not assigned!");
            return;
        }

        if (isSafetyOn)
        {
            targetRenderer.material = defaultMaterial;
        }
        else
        {
            targetRenderer.material = safetyOnMaterial;
        }

        isSafetyOn = !isSafetyOn;
    }
}