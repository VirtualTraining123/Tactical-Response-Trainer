using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Weapon : MonoBehaviour {
  [SerializeField] protected float shootingForce = 50;
  [SerializeField] protected Transform bulletSpawn;
  [SerializeField] private float recoilForce;
  [SerializeField] private float damage;
  [SerializeField] protected AudioManager audioManager;

  private Rigidbody rigidBody;
  private XRGrabInteractable interactableWeapon;
  public InputActionProperty aButtonAction;
  public InputActionProperty bButtonAction;

  protected virtual void Awake() {
    interactableWeapon = GetComponent<XRGrabInteractable>();
    rigidBody = GetComponent<Rigidbody>();
    SetupInteractableWeaponEvents();

    aButtonAction.action.performed += _ => Reload();
    bButtonAction.action.performed += _ => ToggleSafety();
  }

  private void SetupInteractableWeaponEvents() {
    interactableWeapon.onSelectEntered.AddListener(PickUpWeapon);
    interactableWeapon.onSelectExited.AddListener(DropWeapon);
    interactableWeapon.onActivate.AddListener(StartShooting);
    interactableWeapon.onDeactivate.AddListener(StopShooting);
  }

  private void PickUpWeapon(XRBaseInteractor interactor) {
    interactor.GetComponent<MeshHider>().Hide();
  }

  private void DropWeapon(XRBaseInteractor interactor) {
    interactor.GetComponent<MeshHider>().Show();
  }

  protected virtual void StartShooting(XRBaseInteractor interactor)
  {
  }

  protected virtual void StopShooting(XRBaseInteractor interactor)
  {
  }

  protected virtual void Shoot() {
    ApplyRecoil();
    audioManager.Play("shot");
    //audioManager.Play("shells");
  }

  private void ApplyRecoil() {
    rigidBody.AddRelativeForce(Vector3.back * recoilForce, ForceMode.Impulse);
  }

  public float GetShootingForce() {
    return shootingForce;
  }

  public float GetDamage() {
    return damage;
  }

  protected virtual void Reload() {
    audioManager.Play("reload");
  }

  protected virtual void ToggleSafety() {
    audioManager.Play("dry_shot");
  }
}