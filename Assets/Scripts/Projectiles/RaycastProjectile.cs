using UnityEngine;

namespace Projectiles {
  public class RaycastProjectile : Projectile {
    public override void Launch() {
      base.Launch();
      if (!Physics.Raycast(transform.position, transform.forward, out var hit)) return;
      var damageTakers = hit.collider.GetComponentsInParent<ITakeDamage>();
      foreach (var taker in damageTakers) {
        taker.TakeDamage(Weapon, this, hit.point);
      }
    }
  }
}