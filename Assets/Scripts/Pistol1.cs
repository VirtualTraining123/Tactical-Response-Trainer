using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Pistol1 : Weapon {
  [SerializeField] private Projectile bulletPrefab;
  [SerializeField] private float debugRayDuration = 2f;
  public GameObject bulletHolePrefab;

  private static int shotsFiredPistol;

  private Evaluator evaluator;
  private readonly int maxBullets = 12;
  private int currentBullets;
  public bool isSafetyOn;
  private XRBaseInteractor currentInteractor;

  protected void Start() {
    currentBullets = 12;
    aButtonAction.action.Enable();
    bButtonAction.action.Enable();
  }


  protected override void StartShooting(XRBaseInteractor interactor) {
    if (currentBullets <= 0) {
      audioManager.Play("dry_shot");
    } else if (isSafetyOn) {
      audioManager.Play("safety");
    } else {
      base.StartShooting(interactor);
      currentInteractor = interactor;
      DrawDebugRaycast();
      Shoot();
    }
  }

  protected override void Shoot() {
    if (!isSafetyOn) {
      if (currentBullets <= 0) {
        audioManager.Play("dry_shot");
        return;
      }
    } else {
      audioManager.Play("safety");
      return;
    }

    base.Shoot();
    shotsFiredPistol++;
    currentBullets--;


    var projectileInstance = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
    projectileInstance.Init(this);
    projectileInstance.Launch();

    if (currentInteractor != null) {
      SendHapticImpulse(currentInteractor);
    }

    if (Physics.Raycast(bulletSpawn.position, bulletSpawn.forward, out var hit)) {
      if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Civil") ||
          hit.collider.CompareTag("Bullet")) {
        return;
      }

      InstantiateBulletHole(hit.point, hit.normal, hit.collider.transform);
    }
    // evaluator.BulletUsed();
  }

  private void InstantiateBulletHole(Vector3 position, Vector3 normal, Transform parent) {
    var rotation = Quaternion.FromToRotation(Vector3.up, normal);

    var bulletHoleInstance = Instantiate(bulletHolePrefab, position, rotation);
    bulletHoleInstance.transform.localScale = new Vector3(0.2f, 0.005f, 0.2f);
    bulletHoleInstance.transform.SetParent(parent, true);
  }

  private void DrawDebugRaycast() {
    Debug.DrawRay(bulletSpawn.position, bulletSpawn.forward * 100f, Color.yellow, debugRayDuration);
  }

  protected override void Reload() {
    base.Reload();

    currentBullets = maxBullets;
  }

  public void CallReload() {
    Reload();
  }

  public int GetBullets() {
    return currentBullets;
  }

  protected override void ToggleSafety() {
    base.ToggleSafety();

    isSafetyOn = !isSafetyOn;
  }

  public void CallToggleSafety() {
    ToggleSafety();
  }

  private void SendHapticImpulse(XRBaseInteractor interactor) {
    var controllerInteractor = interactor as XRBaseControllerInteractor;
    if (controllerInteractor != null) {
      var controller = controllerInteractor.xrController;
      if (controller != null) {
        controller.SendHapticImpulse(1f, 0.3f); // Ajusta la intensidad y la duración según sea necesario
      }
    }
  }


  private void OnGUI() {
    GUI.Label(new Rect(40, 90, 200, 20), "Disparos realizados: " + shotsFiredPistol);
  }
}