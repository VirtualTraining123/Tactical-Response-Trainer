using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ToggleGrab : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private bool isGrabbed = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Suscribirse a los eventos de agarre y liberación
        grabInteractable.onSelectEntered.AddListener(OnGrab);
        grabInteractable.onSelectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        // Cancelar la suscripción a los eventos
        grabInteractable.onSelectEntered.RemoveListener(OnGrab);
        grabInteractable.onSelectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(XRBaseInteractor interactor)
    {
        // Alternar el estado de agarre
        if (!isGrabbed)
        {
            // Si no está agarrado, mantener el agarre
            isGrabbed = true;
        }
        else
        {
            // Si ya está agarrado, forzar la liberación
            grabInteractable.interactionManager.SelectExit(interactor, grabInteractable);
            isGrabbed = false;
        }
    }

    private void OnRelease(XRBaseInteractor interactor)
    {
        // Reiniciar el estado de agarre al soltar
        isGrabbed = false;
    }
}
