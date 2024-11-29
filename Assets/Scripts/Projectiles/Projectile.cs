using UnityEngine;

namespace Projectiles {
  public class Projectile : MonoBehaviour {
    protected Weapon Weapon;

    public virtual void Init(Weapon weapon) {
      Weapon = weapon;
    }

    public virtual void Launch() { }
  }
}