using UnityEngine;

namespace Projectiles
{
  public class ClientProjectile : BaseProjectile
  {
    public override void Launch()
    {
      GetComponent<Rigidbody>().linearVelocity = transform.forward * speed;
      HandleTrail();
    }
    protected override void HandleTrail()
    {
      if (bulletTrailPrefab == null) return;
      bulletTrail = Instantiate(bulletTrailPrefab, transform.position, Quaternion.identity);
      bulletTrail.transform.SetParent(transform); // Hacer que el rastro siga la bala
    }
  }
}
