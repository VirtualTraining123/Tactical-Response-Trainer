using DefaultNamespace;
using UnityEngine;

public class ColiderRecarga : MonoBehaviour {
  [SerializeField] private Pistol pistol;

  private void OnTriggerEnter(Collider collision) {
    if (!collision.gameObject.CompareTag(ObjectTag.Magazine.ToString())) return;
    pistol.CallReload();
    Destroy(collision.gameObject);
  }
}