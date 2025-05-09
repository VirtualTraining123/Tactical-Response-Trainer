using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
  [SerializeField]
  private List<GameObject> OutsideMeshes;
  [SerializeField]
  private List<GameObject> InsideMeshes;
  [SerializeField]
  private List<GameObject> IntersectingMeshes;

  private void Awake() {
    // Assert IntersectingMeshes is a subset of OutsideMeshes
    if (IntersectingMeshes.Any(mesh => !OutsideMeshes.Contains(mesh))) {
      Debug.LogError("IntersectingMeshes must be a subset of OutsideMeshes");
    }
  }

  public void DisplayOutside() {
    // Show outside meshes
    foreach (GameObject mesh in OutsideMeshes) {
      mesh.SetActive(true);
    }
  }
  public void HideOutside() {
    // Hide outside meshes
    foreach (GameObject mesh in OutsideMeshes) {
      mesh.SetActive(false);
    }
  }
  public void DisplayInside() {
    // Show inside meshes
    foreach (GameObject mesh in InsideMeshes) {
      mesh.SetActive(true);
    }
  }
  public void HideInside() {
    // Hide inside meshes
    foreach (GameObject mesh in InsideMeshes) {
      mesh.SetActive(false);
    }
  }
  public void DisplayIntersecting() {
    // Show intersecting meshes
    foreach (GameObject mesh in IntersectingMeshes) {
      mesh.SetActive(true);
    }
  }
  public void HideIntersecting() {
    // Hide intersecting meshes
    foreach (GameObject mesh in IntersectingMeshes) {
      mesh.SetActive(false);
    }
  }

  public void ToOutside() {
    DisplayOutside();
    HideInside();
  }

  public void ToTransition() {
    DisplayOutside();
    DisplayInside();
    HideIntersecting();
  }

  public void ToInside() {
    HideOutside();
    DisplayInside();
  }
}
