using UnityEngine;

public class RaycastProjectile : Projectile
{
    public override void Launch()
    {
        base.Launch();
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            ITakeDamage[] damageTakers = hit.collider.GetComponentsInParent<ITakeDamage>();
            foreach (var taker in damageTakers)
            {
                taker.TakeDamage(Weapon, this, hit.point);
            }
        }
    }
}