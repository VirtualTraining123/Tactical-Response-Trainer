using UnityEngine;
using UnityEngine.InputSystem;

public class HandsAnimationController : MonoBehaviour
{
    [SerializeField] private Animator handAnimator;
    [SerializeField] private InputAction gripAction;
    [SerializeField] private InputAction triggerAction;
    [SerializeField] private string nHand = "";

    private void Awake()
    {
        gripAction.performed += GripPressed;
        triggerAction.performed += TriggerPressed;
    }

    private void TriggerPressed(InputAction.CallbackContext obj)
    {
        var aux = "Select_";
        handAnimator.SetFloat(aux+nHand, obj.ReadValue<float>());
    }

    private void GripPressed(InputAction.CallbackContext obj)
    {
        var aux = "Grab_";
        handAnimator.SetFloat(aux+nHand, obj.ReadValue<float>());
    }
    
    private void OnEnable()
    {
        gripAction.Enable();
        triggerAction.Enable();
    }

    private void OnDisable()
    {
        gripAction.Disable();
        triggerAction.Disable();
    }
}