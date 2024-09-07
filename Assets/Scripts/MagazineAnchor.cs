using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class MagazineAnchor : MonoBehaviour {
  public Transform anchorPoint; // Punto de anclaje en el chaleco

  private Rigidbody rb;
  private XRGrabInteractable grabInteractable;
  private bool isHeld;

  [Obsolete("Obsolete")]
  void Awake() {
    rb = GetComponent<Rigidbody>();
    grabInteractable = GetComponent<XRGrabInteractable>();

    grabInteractable.onSelectEntered.AddListener(OnGrab);
    grabInteractable.onSelectExited.AddListener(OnRelease);
    isHeld = false;
    rb.isKinematic = true;
  }

  void Start() {
    AnchorMagazine();
  }

  void FixedUpdate() {
    if (!isHeld) {
      // Mantener el cargador en el punto de anclaje cuando no se sostiene
      rb.MovePosition(anchorPoint.position);
      rb.MoveRotation(anchorPoint.rotation);
    }
  }

  private void OnGrab(XRBaseInteractor interactor) {
    isHeld = true;
    rb.isKinematic = false;

    // Cambiar la etiqueta al momento de tomarlo
    // gameObject.tag = "HeldMagazine";
  }

  private void OnRelease(XRBaseInteractor interactor) {
    isHeld = false;
    rb.isKinematic = true; // Desactivar la física mientras está anclado
    AnchorMagazine();
  }

  private void AnchorMagazine() {
    // Mover el cargador al punto de anclaje
    transform.position = anchorPoint.position;
    transform.rotation = anchorPoint.rotation;
  }
}