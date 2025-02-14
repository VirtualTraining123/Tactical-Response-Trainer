using System;
using Projectiles;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Pistol : Weapon {
  private static readonly int PistolShoot = Animator.StringToHash("PistolShoot");
  [SerializeField] private Projectile bulletPrefab;
  [SerializeField] private float debugRayDuration = 2f;
  [SerializeField] private bool isEvaluated = true;
  [SerializeField] private int maxBullets = 100;
  [SerializeField] private Animator pistolAnimator;

  private Evaluator evaluator;
  private int currentBullets;
  public bool isSafetyOn;
  private XRBaseInteractor currentInteractor;

  protected void Start() {
    currentBullets = maxBullets;
    if (isEvaluated)  evaluator = FindFirstObjectByType<Evaluator>();
  }

  public void OnPistolAnimationEnd(AnimationEvent eventInfo) {
    if (eventInfo.animatorClipInfo.clip.name == "PistolShoot") {
      pistolAnimator.SetBool(PistolShoot, false);
    } else {
      Debug.LogWarning("Animation event not found.");
    }
  }

  [Obsolete("Obsolete")]
  protected override void StartShooting(ActivateEventArgs interactor) {
    currentInteractor = interactor.interactor;
    Shoot();
  }

  protected override void Shoot() {
    if (isSafetyOn) {
      SafetyStillActiveSound();
      return;
    }

    if (currentBullets <= 0) {
      ShotNoBulletsSound();
      return;
    }

    base.Shoot();
    //llamamos al animador de pistol y modificamos el parametro PistolShoot de 0 a 1 y luego de 1 a 0 para volver a idle
    pistolAnimator.SetBool(PistolShoot, true);
    evaluator?.OnBulletUsed();
    currentBullets--;

    var projectileInstance = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
    projectileInstance.Init(this);
    projectileInstance.Launch();

    if (currentInteractor != null) SendHapticImpulse(currentInteractor);
  }

  // ReSharper disable once UnusedMember.Local
  private void OnSelectAction() {
    // no se elimina porque funciona con los botones del mando y podria servir, quizas?
    OnToggleSafety();
  }

  // ReSharper disable once UnusedMember.Local
  private void OnActivateAction() {
    OnReload();
  }

  public override void OnReload() {
    base.OnReload();
    currentBullets = maxBullets;
  }

  public override void OnToggleSafety() {
    base.OnToggleSafety();
    isSafetyOn = !isSafetyOn;
  }

  private static void SendHapticImpulse(XRBaseInteractor interactor) {
    var controllerInteractor = interactor as XRBaseInputInteractor;
    if (controllerInteractor == null) return;
    var controller = controllerInteractor.xrController;
    if (controller == null) return;
    controller.SendHapticImpulse(1f, 0.3f);
  }
}