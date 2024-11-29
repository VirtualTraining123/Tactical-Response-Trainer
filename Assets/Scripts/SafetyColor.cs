using UnityEngine;

public class SafetyColor : MonoBehaviour {
  /// The Renderer of the piece that will change material
  [SerializeField] private Renderer targetRenderer;
  [SerializeField] private Material safetyOffMaterial;
  [SerializeField] private Material safetyOnMaterial;

  [SerializeField] private Pistol pistol;

  public void UpdateMaterial() {
    if (targetRenderer == null) {
      Debug.LogWarning("Target Renderer not assigned!");
      return;
    }
    targetRenderer.material = pistol.isSafetyOn ? safetyOnMaterial : safetyOffMaterial;
  }
}