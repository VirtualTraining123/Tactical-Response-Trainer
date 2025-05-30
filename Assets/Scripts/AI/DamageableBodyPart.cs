using Projectiles;
using UnityEngine;
namespace AI {
  public class DamageableBodyPart : DamageablePart<BodyPart> {
    [SerializeField] public BodyMasterDamageable Master;
    public override void TakeDamage(Weapon weapon, Projectile projectile, Vector3 contactPoint) {
      Master.TakeDamage(weapon, projectile, contactPoint, name);
    }
  }
}
