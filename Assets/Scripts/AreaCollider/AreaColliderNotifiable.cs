using UnityEngine;
namespace AreaCollider {
  public abstract class AreaColliderNotifiable : MonoBehaviour {

    public abstract void OnTransitionListener(AreaCollider coll, Collider other);
  }
}
