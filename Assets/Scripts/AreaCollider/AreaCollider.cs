using System.Collections.Generic;
using UnityEngine;
namespace AreaCollider {
  [RequireComponent(typeof(Collider))]
  public class AreaCollider: MonoBehaviour {
    [SerializeField]
    private Collider collider;
    [SerializeField]
    private AreaColliderName name;
    [SerializeField]
    private AreaColliderNotifiable notifiable;
    private readonly List<Collider> playerInside = new();  

    public AreaColliderName GetName() {
      return name;
    }
    public List<Collider> GetPlayerInside() {
      return playerInside;
    }
    private void Awake() {
      collider = GetComponent<Collider>();
    }
    private void OnTriggerEnter(Collider other) {
      if (!other.CompareTag("MainCamera")) return;
      playerInside.Add(other);
      notifiable.OnTransitionListener(this, other);
      Debug.Log($"Player entered the area {name}");
    }

    private void OnTriggerExit(Collider other) {
      if (!other.CompareTag("MainCamera")) return;
      playerInside.Remove(other);
      notifiable.OnTransitionListener(this, other);
      Debug.Log($"Player exited the area {name}");
    }

  }
}
