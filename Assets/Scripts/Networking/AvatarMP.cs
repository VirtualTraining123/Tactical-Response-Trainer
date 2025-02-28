using UnityEngine;
using RootMotion.FinalIK;

public class AvatarMp : MonoBehaviour
{
    // Referencia al componente VRIK que debe estar en el avatar.
    private VRIK vrik;

    private void Awake() {
        // Se obtiene el componente VRIK.
        vrik = GetComponent<VRIK>();
        if (vrik == null) {
            Debug.LogError("No se encontró el componente VRIK en el avatar.");
        }
    }

    /// <summary>
    /// Asigna el target para la mano izquierda al VRIK.
    /// </summary>
    /// <param name="target">Transform de la mano izquierda (player marker).</param>
    public void SetLeftHandTarget(Transform target) {
        if (vrik != null) {
            vrik.references.leftHand = target;
            // Si requieres actualizar otros componentes o forzar la recalculación del solver, lo puedes hacer aquí.
        }
    }

    /// <summary>
    /// Asigna el target para la mano derecha al VRIK.
    /// </summary>
    /// <param name="target">Transform de la mano derecha (player marker).</param>
    public void SetRightHandTarget(Transform target) {
        if (vrik != null) {
            vrik.references.rightHand = target;
        }
    }

    /// <summary>
    /// Asigna el target para la cabeza (gaze) al VRIK.
    /// </summary>
    /// <param name="target">Transform del gaze (player marker) que se utilizará como cabeza.</param>
    public void SetHeadTarget(Transform target) {
        if (vrik != null) {
            vrik.references.head = target;
        }
    }
}