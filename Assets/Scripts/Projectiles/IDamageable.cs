using Projectiles;
using System;
using UnityEngine;
public interface IDamageable<in PartTypeList> where PartTypeList : Enum {
  void TakeDamage(Weapon weapon, Projectile projectile, Vector3 contactPoint, PartTypeList @enum);
}
