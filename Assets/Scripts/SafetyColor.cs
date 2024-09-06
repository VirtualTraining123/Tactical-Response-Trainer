using UnityEngine;

public class SafetyColor : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer; // El Renderer de la pieza que cambiará de material
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Material safetyOnMaterial;

     [SerializeField] private bool isSafetyOn;

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