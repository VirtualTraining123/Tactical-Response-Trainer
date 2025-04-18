using Projectiles;
using UnityEngine;

public interface ITakeDamage
{
    void TakeDamage(Weapon weapon, BaseProjectile projectile, Vector3 contactPoint);
}
