using Projectiles;
using UnityEngine;
namespace AI {
  public abstract class BodyMasterDamageable : MonoBehaviour, IDamageable<BodyPart> {
    public abstract void TakeDamage(Weapon weapon, Projectile projectile, Vector3 contactPoint, BodyPart @enum);
  }
}
