using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsProjectile : Projectile
{
    [SerializeField] private float lifeTime;
    private Rigidbody rigidBody;
    public GameObject bulletHolePrefab;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public override void Init(Weapon weapon)
    {
        base.Init(weapon);
        Destroy(gameObject, lifeTime);
    }

    public override void Launch()
    {
        base.Launch();
        rigidBody.AddRelativeForce(Vector3.forward * weapon.GetShootingForce(), ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Destroy the projectile
        Destroy(gameObject);

        // Get the point of contact
        Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);

        // Instantiate the bullet hole at the contact point
      

        // Deal damage to objects in the vicinity (if any)
        ITakeDamage[] damageTakers = other.GetComponentsInParent<ITakeDamage>();
        foreach (var taker in damageTakers)
        {
            taker.TakeDamage(weapon, this, contactPoint);
        }
    }
}
