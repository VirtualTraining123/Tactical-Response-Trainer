using DefaultNamespace;
using UnityEngine;

public class ReloadCollider : MonoBehaviour {
  [SerializeField] private Pistol pistol;

  private void OnTriggerEnter(Collider collision) {
    if (!collision.gameObject.CompareTag(ObjectTag.Magazine.ToString())) return;
    pistol.OnReload();
    Destroy(collision.gameObject);
  }
}