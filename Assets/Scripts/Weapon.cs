using Audio;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class Weapon : MonoBehaviour {
  [SerializeField] protected float shootingForce = 50;
  [SerializeField] protected Transform bulletSpawn;
  [SerializeField] private float recoilForce;
  [SerializeField] private float damage;

  private Rigidbody rigidBody;
  private XRGrabInteractable interactableWeapon;
  protected readonly InlineAudioManager InlineAudioManager = new();


  protected virtual void Awake() {
    interactableWeapon = GetComponent<XRGrabInteractable>();
    rigidBody = GetComponent<Rigidbody>();
    SetupInteractableWeaponEvents();
    InlineAudioManager.Scan(this);
    InlineAudioManager.RequestAudioSources(
      ClipName.GUNSHOT,
      ClipName.BULLET_SHELLS,
      ClipName.RELOAD,
      ClipName.SAFETY_TOGGLE,
      ClipName.DRY_FIRE
    );
  }

  private void SetupInteractableWeaponEvents() {
    interactableWeapon.selectEntered.AddListener(PickUpWeapon);
    interactableWeapon.selectExited.AddListener(DropWeapon);
    interactableWeapon.activated.AddListener(StartShooting);
    interactableWeapon.deactivated.AddListener(StopShooting);
  }

  private static void PickUpWeapon(SelectEnterEventArgs interactor) {
    var meshHider = interactor.interactorObject.transform.GetComponent<MeshHider>();
    if (meshHider) {
      meshHider.Hide();
    } else {
      Debug.LogWarning("MeshHider component missing on interactor.");
    }
  }

  private static void DropWeapon(SelectExitEventArgs interactor) {
    var meshHider = interactor.interactorObject.transform.GetComponent<MeshHider>();
    if (meshHider) {
      meshHider.Show();
    } else {
      Debug.LogWarning("MeshHider component missing on interactor.");
    }
  }

  protected virtual void StartShooting(ActivateEventArgs interactor) { }

  protected virtual void StopShooting(DeactivateEventArgs interactor) { }

  protected virtual void Shoot() {
    ApplyRecoil();
    InlineAudioManager.GetAudioSource(ClipName.GUNSHOT).Play();
    InlineAudioManager.GetAudioSource(ClipName.BULLET_SHELLS).Play();
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
    InlineAudioManager.GetAudioSource(ClipName.RELOAD).Play();
  }

  protected void ToggleSafetySound() {
    InlineAudioManager.GetAudioSource(ClipName.SAFETY_TOGGLE).Play();
  }

  protected void SafetyStillActiveSound() {
    InlineAudioManager.GetAudioSource(ClipName.DRY_FIRE).Play();
  }


  protected void ShotNoBulletsSound() {
    InlineAudioManager.GetAudioSource(ClipName.DRY_FIRE).Play();
  }
}
