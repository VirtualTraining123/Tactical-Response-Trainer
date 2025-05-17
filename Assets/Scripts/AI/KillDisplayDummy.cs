using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
namespace AI {
  public class KillDisplayDummy : MonoBehaviour {
    private Dictionary<BodyPart, DamageableBodyPart> bodyParts = new();
    [SerializeField] public Material undamagedMaterial;
    [SerializeField] public Material damagedMaterial;
    private void Awake() {
      // Populate bodyParts dictionary with all DamageableBodyPart components in the children
      foreach (var part in GetComponentsInChildren<DamageableBodyPart>()) {
        bodyParts[part.name] = part;
      }
    }

    public void DisplayDamagedPart(BodyPart part) {
      // Assign undamaged material to all parts except the specified one
      foreach (var bodyPart in bodyParts) {
        try {
          if (bodyPart.Value.gameObject.GetNamedChild("bone_display").TryGetComponent<Renderer>(out var component)) {
            component.material = bodyPart.Key == part ? damagedMaterial : undamagedMaterial;
            Debug.Log($"{bodyPart} OK.");
          } else {
            Debug.Log($"Failed to get component for {bodyPart}.");
          }
        } catch (NullReferenceException) {
          Debug.Log($"Failed to get component for {bodyPart}.");

        }
      }
    }
  }
}
