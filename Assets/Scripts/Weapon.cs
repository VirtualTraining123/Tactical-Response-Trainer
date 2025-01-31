using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Weapon : MonoBehaviour {
  [SerializeField] protected float shootingForce = 50;
  [SerializeField] protected Transform bulletSpawn;
  [SerializeField] private float recoilForce;
  [SerializeField] private float damage;
  private AudioManager audioManager;

  private Rigidbody rigidBody;
  private XRGrabInteractable interactableWeapon;
  public InputActionProperty aButtonAction;
  public InputActionProperty bButtonAction;

  protected virtual void Awake() {
    interactableWeapon = GetComponent<XRGrabInteractable>();
    rigidBody = GetComponent<Rigidbody>();
    SetupInteractableWeaponEvents();
    audioManager = FindObjectOfType<AudioManager>();

    aButtonAction.action.performed += _ => ReloadSound();
    bButtonAction.action.performed += _ => ToggleSafetySound();

    audioManager.Request("shot", gameObject);
    audioManager.Request("shells", gameObject);
    audioManager.Request("reload", gameObject);
    audioManager.Request("toggle_safety", gameObject);
    audioManager.Request("safety", gameObject);
    audioManager.Request("dry_shot", gameObject);
  }

  private void SetupInteractableWeaponEvents() {
    interactableWeapon.selectEntered.AddListener(PickUpWeapon);
    interactableWeapon.selectExited.AddListener(DropWeapon);
    interactableWeapon.activated.AddListener(StartShooting);
    interactableWeapon.deactivated.AddListener(StopShooting);
  }

  private static void PickUpWeapon(SelectEnterEventArgs interactor) {
    var meshHider = interactor.interactorObject.transform.GetComponent<MeshHider>();
    if (meshHider != null) {
      meshHider.Hide();
    } else {
      Debug.LogWarning("MeshHider component missing on interactor.");
    }
  }

  private static void DropWeapon(SelectExitEventArgs interactor) {
    var meshHider = interactor.interactorObject.transform.GetComponent<MeshHider>();
    if (meshHider != null) {
      meshHider.Show();
    } else {
      Debug.LogWarning("MeshHider component missing on interactor.");
    }
  }

  protected virtual void StartShooting(ActivateEventArgs interactor) { }

  protected virtual void StopShooting(DeactivateEventArgs interactor) { }

  protected virtual void Shoot() {
    ApplyRecoil();
    audioManager.Play("shot", gameObject);
    audioManager.Play("shells", gameObject);
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

  public virtual void OnToggleSafety() {
    ToggleSafetySound();
  }


  public virtual void OnReload() {
    ReloadSound();
  }

  protected void ReloadSound() {
    audioManager.Play("reload", gameObject);
  }

  protected void ToggleSafetySound() {
    audioManager.Play("toggle_safety", gameObject);
  }

  protected void SafetyStillActiveSound() {
    audioManager.Play("safety", gameObject);
  }


  protected void ShotNoBulletsSound() {
    audioManager.Play("dry_shot", gameObject);
  }
}