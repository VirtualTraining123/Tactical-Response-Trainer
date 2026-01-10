using UnityEngine;

namespace Projectiles {
  public class RaycastProjectile : BaseProjectile {
    public override void Launch() {
      if (!Physics.Raycast(transform.position, transform.forward, out var hit)) return;
      var damageTakers = hit.collider.GetComponentsInParent<ITakeDamage>();
      foreach (var taker in damageTakers) {
        taker.TakeDamage(Weapon, this, hit.point);
      }
    }

    protected override void HandleTrail() {
      // No trail for raycast projectiles
    }
  }
}