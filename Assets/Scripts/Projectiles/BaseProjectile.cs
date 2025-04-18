using UnityEngine;

namespace Projectiles
{
    public abstract class BaseProjectile : MonoBehaviour
    {
        protected Weapon Weapon;
        public float speed = 150f;
        public GameObject bulletTrailPrefab;
        protected GameObject bulletTrail;

        public virtual void Init(Weapon weapon)
        {
            Weapon = weapon;
        }

        public abstract void Launch();
        protected abstract void HandleTrail();
        private void OnCollisionEnter(Collision collision)
        {
            var contact = collision.GetContact(0);
            HandleHit(contact, collision.transform);
            Destroy(gameObject);
        }
        private void HandleHit(ContactPoint contact, Transform hitTransform)
        {
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
