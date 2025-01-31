using UnityEngine;

namespace Projectiles {
  public class Projectile : MonoBehaviour {
    protected Weapon Weapon;
    private float speed = 150f;
    public GameObject bulletTrailPrefab;
    private GameObject bulletTrail;

    public virtual void Init(Weapon weapon) {
      Weapon = weapon;
    }
    
    public virtual void Launch() {
      GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
      
      // Instanciar el rastro de bala
      if (bulletTrailPrefab == null) return;
      bulletTrail = Instantiate(bulletTrailPrefab, transform.position, Quaternion.identity);
      bulletTrail.transform.SetParent(transform); // Hacer que el rastro siga la bala
    }
    
    private void OnCollisionEnter(Collision collision) {
      var contact = collision.GetContact(0);
      HandleHit(contact, collision.transform);
      Destroy(gameObject);
    }

    protected virtual void HandleHit(ContactPoint contact, Transform hitTransform) {
      // Create a bullet hole where the projectile hit
      BulletHoleManager.Instance.CreateBulletHole(
          contact.point,
          contact.normal,
          hitTransform
      );

      // Check if the hit object can take damage
      var damageable = hitTransform.GetComponent<ITakeDamage>();
      damageable?.TakeDamage(Weapon, this, contact.point);
    }
  }
}
