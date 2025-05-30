using System;
using UnityEngine;
namespace Projectiles {
  [RequireComponent(typeof(Collider))]
  public abstract class DamageablePart<PartTypeList> : MonoBehaviour, ITakeDamage where PartTypeList : Enum {
    [SerializeField] public new PartTypeList name;


    public abstract void TakeDamage(Weapon weapon, Projectile projectile, Vector3 contactPoint);
  }
}
