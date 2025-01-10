using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class MagazineAnchor : MonoBehaviour {
  public Transform anchorPoint;

  private Rigidbody rb;
  private XRGrabInteractable grabInteractable;
  private bool isHeld;

  public void Awake() {
    rb = GetComponent<Rigidbody>();
    grabInteractable = GetComponent<XRGrabInteractable>();

    grabInteractable.selectEntered.AddListener(OnGrab);
    grabInteractable.selectExited.AddListener(OnRelease);
    isHeld = false;
    rb.isKinematic = true;
  }

  private void Start() {
    MoveToAnchor();
  }

  public void FixedUpdate() {
    if (isHeld) return;
    MoveToAnchor();
  }

  private void OnGrab(SelectEnterEventArgs interactor) {
    isHeld = true;
    rb.isKinematic = false;
  }

  private void OnRelease(SelectExitEventArgs interactor) {
    isHeld = false;
    rb.isKinematic = true;
    MoveToAnchor();
  }

  private void MoveToAnchor() {
    rb.Move(anchorPoint.position, anchorPoint.rotation);
  }
}