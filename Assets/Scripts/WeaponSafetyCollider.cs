using DefaultNamespace;
using UnityEngine;

public class WeaponSafetyCollider : MonoBehaviour {
  [SerializeField] private SafetyColor safetyColor;
  [SerializeField] private Pistol pistol;
  [SerializeField] private float safetyCooldownDuration = 0.5f;
  [SerializeField] private ObjectTag triggerTag = ObjectTag.RightHand;

  private float lastCooldownTime;

  private void OnTriggerEnter(Collider collision) {
    if (!collision.gameObject.CompareTag(triggerTag.ToString())) return;
    if (Time.time - lastCooldownTime < safetyCooldownDuration) return;
    lastCooldownTime = Time.time;
    pistol.CallToggleSafety();
    safetyColor.ToggleMaterial();
  }
}