using System;
using System.Collections.Generic;
using System.Linq;
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
      DisplayDamagedParts(bodyPart => bodyPart == part);
    }

    public void DisplayDamagedParts(List<BodyPart> parts) {
      DisplayDamagedParts(parts.Contains);
    }
    
    public void DisplayDamagedParts(Predicate<BodyPart> predicate) {
      // Assign undamaged material to all parts except the specified ones
      foreach (var bodyPart in bodyParts) {
        try {
          if (bodyPart.Value.gameObject.GetNamedChild("bone_display").TryGetComponent<Renderer>(out var component)) {
            component.material = predicate.Invoke(bodyPart.Key) ? damagedMaterial : undamagedMaterial;
          }
        } catch (NullReferenceException) {
          // Pass
        }
      }
    }

  }
}
