using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class WeaponAnchor : MonoBehaviour
{
    public Transform anchorPoint; // Punto de anclaje en el chaleco
    public float moveSpeed = 1f; // Velocidad de movimiento hacia el anclaje
    public float grabSmoothTime = 0.2f; // Tiempo de suavizado al agarrar

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private bool isHeld;
    private Vector3 initialScale;

    private Coroutine smoothGrabCoroutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Suscribirse a los eventos de XRGrabInteractable
        grabInteractable.onSelectEntered.AddListener(OnGrab);
        grabInteractable.onSelectExited.AddListener(OnRelease);

        // Inicialmente, el arma no está sostenida
        isHeld = false;

        // Configurar el Rigidbody
        rb.isKinematic = true;

        // Guardar la escala inicial
       // initialScale = transform.localScale;
    }

    void Start()
    {
        AnchorWeapon();
    }

    void FixedUpdate()
    {
        if (!isHeld)
        {
            // Movimiento suave hacia el punto de anclaje
            Vector3 newPosition = Vector3.MoveTowards(transform.position, anchorPoint.position, moveSpeed * Time.fixedDeltaTime);
            Quaternion newRotation = Quaternion.Lerp(transform.rotation, anchorPoint.rotation, moveSpeed * Time.fixedDeltaTime);

            rb.MovePosition(newPosition);
            rb.MoveRotation(newRotation);

            // Asegurar que el arma conserve su escala original
           // transform.localScale = initialScale;
        }
    }

    private void OnGrab(XRBaseInteractor interactor)
    {
        if (smoothGrabCoroutine != null)
        {
            StopCoroutine(smoothGrabCoroutine);
        }

        smoothGrabCoroutine = StartCoroutine(SmoothGrab(interactor));
    }

    private void OnRelease(XRBaseInteractor interactor)
    {
        isHeld = false;
        rb.isKinematic = true; // Desactivar la física mientras está anclado
    }

    private void AnchorWeapon()
    {
        // Mover el arma al punto de anclaje
        transform.position = anchorPoint.position;
        transform.rotation = anchorPoint.rotation;
      //  transform.localScale = initialScale; // Restablecer la escala original
    }

    private IEnumerator SmoothGrab(XRBaseInteractor interactor)
    {
        isHeld = true;
        rb.isKinematic = false;

        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < grabSmoothTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / grabSmoothTime;

            transform.position = Vector3.Lerp(initialPosition, interactor.transform.position, t);
            transform.rotation = Quaternion.Lerp(initialRotation, interactor.transform.rotation, t);

            yield return null;
        }

        // Asegurar que el arma esté completamente en la posición y rotación del interactor al final
        transform.position = interactor.transform.position;
        transform.rotation = interactor.transform.rotation;
    }
}
