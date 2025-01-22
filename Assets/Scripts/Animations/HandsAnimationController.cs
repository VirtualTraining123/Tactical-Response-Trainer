using UnityEngine;
using UnityEngine.InputSystem;

namespace Animations
{
    public class HandsAnimationController : MonoBehaviour
    {
        private static readonly int Pistol1 = Animator.StringToHash("Pistol");
        private static readonly int Shooting = Animator.StringToHash("Shooting");
        [SerializeField] private Animator handAnimator;
        [SerializeField] private InputAction gripAction;
        [SerializeField] private InputAction triggerAction;
        [SerializeField] private string nHand = "";

        [SerializeField] private LayerMask interactableLayer; // Capa para objetos interactuables
        private GameObject currentWeapon; // Referencia al arma tomada
        private bool isHoldingPistol;

        private void Awake()
        {
            gripAction.performed += GripPressed;
            gripAction.canceled += GripReleased; // Detecta cuando se suelta el botón de agarre
            triggerAction.performed += TriggerPressed;
        }

        private void GripPressed(InputAction.CallbackContext obj)
        {
            if (!isHoldingPistol)
            {
                // Detectar si se toma un objeto interactuable
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.1f, interactableLayer);
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.CompareTag("Weapon"))
                    {
                        // Se detectó un arma
                        isHoldingPistol = true;
                        currentWeapon = hitCollider.gameObject;
                        handAnimator.SetBool(Pistol1, true); //revisar, falta aux+nHand para que no anime ambas manos?

                        // Opcional: fijar el arma a la mano //no lo veo necesario
                        currentWeapon.transform.SetParent(transform);
                        currentWeapon.transform.localPosition = Vector3.zero;
                        currentWeapon.transform.localRotation = Quaternion.identity;
                        currentWeapon.GetComponent<Rigidbody>().isKinematic = true;

                        break;
                    }
                    else
                    {
                        var aux = "Grab_";
                        handAnimator.SetFloat(aux + nHand, obj.ReadValue<float>());

                    }
                }
            }
        }

        private void GripReleased(InputAction.CallbackContext obj)
        {
            if (isHoldingPistol)
            {
                // Soltar el arma
                isHoldingPistol = false;
                handAnimator.SetBool(Pistol1, false); //revisar, falta aux+nHand para que no anime ambas manos?

                if (currentWeapon != null)
                {
                    // Liberar el arma de la mano
                    currentWeapon.transform.SetParent(null);
                    Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = false;
                    }

                    currentWeapon = null;
                }
            }
        }

        private void TriggerPressed(InputAction.CallbackContext obj)
        {
            if (isHoldingPistol)
            {
                // Controla la animación del disparo
                handAnimator.SetFloat(Shooting, obj.ReadValue<float>()); //revisar, falta aux+nHand para que no anime ambas manos?
            }
            else
            {
                var aux = "Select_";
                handAnimator.SetFloat(aux + nHand, obj.ReadValue<float>()); 
            }
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
}





 