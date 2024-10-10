using UnityEngine;

namespace Projectiles {
  [RequireComponent(typeof(Rigidbody))]
  public class PhysicsProjectile : Projectile {
    [SerializeField] private float lifeTime;
    [SerializeField] private float gravity = 9.81f;
    private float currentGravity;
    private Rigidbody rigidBody;
    public GameObject bulletHolePrefab;
    public GameObject bulletTrailPrefab;
    private GameObject bulletTrail;
    private bool destroying; // Indica si la bala se está destruyendo

    private void Awake() {
      rigidBody = GetComponent<Rigidbody>();
    }

    public override void Init(Weapon pWeapon) {
      base.Init(pWeapon);
      Destroy(gameObject, lifeTime);
    }

    public override void Launch() {
      base.Launch();
      rigidBody.AddRelativeForce(Vector3.forward * Weapon.GetShootingForce(), ForceMode.Impulse);

      // Instanciar el rastro de bala
      if (bulletTrailPrefab == null) return;
      bulletTrail = Instantiate(bulletTrailPrefab, transform.position, Quaternion.identity);
      bulletTrail.transform.SetParent(transform); // Hacer que el rastro siga la bala
    }

    private void FixedUpdate() {
      if (destroying) return;

      currentGravity -= gravity * Time.deltaTime;
      var moveVector = transform.forward * (Weapon.GetShootingForce() * Time.deltaTime) + Vector3.up * currentGravity;

      if (!Physics.Raycast(transform.position, moveVector, out var hit, moveVector.magnitude)) {
        transform.position += moveVector;
        return;
      }
      destroying = true;
      MakeBulletHole(hit, moveVector);
      OnBulletHit(hit);
    }

    private void MakeBulletHole(RaycastHit hit, Vector3 moveVector) {
      if (bulletHolePrefab) {
        var exp = Instantiate(
          bulletHolePrefab,
          hit.point - moveVector.normalized * .01f,
          Quaternion.LookRotation(hit.normal)
        );
        exp.transform.SetParent(hit.transform);
      }
    }

    private void OnBulletHit(RaycastHit hit) {
      // Infligir daño a los objetos en las cercanías (si los hay)
      var contactPoint = hit.point;
      var damageTakers = hit.collider.GetComponentsInParent<ITakeDamage>();
      foreach (var taker in damageTakers) {
        taker.TakeDamage(Weapon, this, contactPoint);
      }
      // Destruir la bala
      Destroy(gameObject);
    }
  }
}