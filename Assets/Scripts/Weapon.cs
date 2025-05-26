using System.Collections.Generic;
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
   private Dictionary<string, AudioSource> audioSources = new();
   
 
   protected virtual void Awake() {
     interactableWeapon = GetComponent<XRGrabInteractable>();
     rigidBody = GetComponent<Rigidbody>();
     SetupInteractableWeaponEvents();
     foreach (var source in GetComponentsInChildren<AudioSource>()) {
       if (!audioSources.TryAdd(source.clip.name, source)) {
         Debug.LogWarning($"Audio clip name conflict: {source.clip.name} already exists.");
       }
     }
    
     
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
     PlaySound("Weapons_SMG_Shoot");
     PlaySound("bulletshells02");
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
     PlaySound("reloadSound");
   }
 
   protected void ToggleSafetySound() {
     PlaySound("Weapon_Safety");
   }
 
   protected void SafetyStillActiveSound() {
     PlaySound("Dry-Pistol-Gunfire-Single-A");
   }
 
 
   protected void ShotNoBulletsSound() {
     PlaySound("Dry-Pistol-Gunfire-Single-A");
   }
   
   private void PlaySound(string clipName) {
     if (audioSources.TryGetValue(clipName, out var source)) {
       source.Play();
     } else {
       Debug.LogWarning($"Audio clip '{clipName}' not found on {gameObject.name}");
     }
   }
   
 }