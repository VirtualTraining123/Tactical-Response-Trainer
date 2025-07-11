using Animations;
using AreaCollider;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class GameStateManager : AreaColliderNotifiable {
  public enum GameState {
    Outside,
    Inside,
    Transition
  }
  [SerializeField]
  private List<GameObject> OutsideMeshes;
  [SerializeField]
  private List<GameObject> InsideMeshes;
  [SerializeField]
  private List<GameObject> IntersectingMeshes;
  [SerializeField] private GameState currentGameState = GameState.Outside;
  [SerializeField]
  [CanBeNull]
  public MovementAnimator outsideAnimator;
  [SerializeField]
  [CanBeNull]
  public MovementAnimator insideAnimator;
  [SerializeField]
  [CanBeNull]
  public MovementAnimator transitionAnimator;
  [SerializeField]
  public AreaCollider.AreaCollider TransitionCollider;
  [SerializeField]
  public AreaCollider.AreaCollider InsiderCollider;
  private bool OutsideAnimatorState = false;
  private bool InsideAnimatorState = false;
  private bool TransitionAnimatorState = false;
  private GameState lastGameState = GameState.Inside;


  private void Awake() {
    // Assert IntersectingMeshes is a subset of OutsideMeshes
    if (IntersectingMeshes.Any(mesh => !OutsideMeshes.Contains(mesh))) {
      Debug.LogError("IntersectingMeshes must be a subset of OutsideMeshes");
    }
  }

  private void Update() {
    if (currentGameState == lastGameState) return;
    switch (currentGameState) {
      case GameState.Inside:
        ToInside();
        break;
      case GameState.Transition:
        ToTransition();
        break;
      case GameState.Outside:
        ToOutside();
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    lastGameState = currentGameState;
  }

  private void DisplayOutside() {
    // Show outside meshes
    foreach (GameObject mesh in OutsideMeshes) {
      mesh.SetActive(true);
    }
  }
  private void HideOutside() {
    // Hide outside meshes
    foreach (GameObject mesh in OutsideMeshes) {
      mesh.SetActive(false);
    }
  }
  private void DisplayInside() {
    // Show inside meshes
    foreach (GameObject mesh in InsideMeshes) {
      mesh.SetActive(true);
    }
  }
  private void HideInside() {
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
  private void HideIntersecting() {
    // Hide intersecting meshes
    foreach (GameObject mesh in IntersectingMeshes) {
      mesh.SetActive(false);
    }
  }

  private void ToOutside() {
    
    outsideAnimator?.SetActive(false);
    insideAnimator?.SetActive(false);
    transitionAnimator?.ClearOnAnimationEndCallbacks();
    transitionAnimator?.SetOnAnimationEnd(endPosition => {
      Debug.Log("Puertas cerradas. Ocultando el interior.");
      HideInside();
    });
    transitionAnimator?.SetActive(false); 
  }

  private void ToInside() {
    outsideAnimator?.SetActive(false);
    insideAnimator?.SetActive(false);
    transitionAnimator?.ClearOnAnimationEndCallbacks();
    transitionAnimator?.SetOnAnimationEnd(endPosition => {
      Debug.Log("Puertas cerradas. Ocultando el exterior.");
      HideOutside();
    });
    transitionAnimator?.SetActive(false);
  }

  private void ToTransition() {
    // La lógica de transición no cambia: muestra todo y abre las puertas.
    DisplayOutside();
    DisplayInside();
    HideIntersecting();
    
    outsideAnimator?.SetActive(false);
    insideAnimator?.SetActive(false);
    
    // Limpiamos callbacks para que no se ejecute una acción de ocultar por error.
    transitionAnimator?.ClearOnAnimationEndCallbacks();
    // Abre las puertas.
    transitionAnimator?.SetActive(true);
  }
  private void ReplaceAnimatorCallbacks(CallbackWaiter<Vector3> waiter) {
    foreach (var animator in new List<MovementAnimator> {
        outsideAnimator,
        insideAnimator,
        transitionAnimator
      }) {
      animator?.ClearOnAnimationEndCallbacks();
      animator?.SetOnAnimationEnd(waiter.CreateCallback());
    }
  }
  // En GameStateManager.cs

  public override void OnTransitionListener(AreaCollider.AreaCollider coll, Collider other) {
    var inside = InsiderCollider.GetPlayerInside().Count > 0;
    var transition = TransitionCollider.GetPlayerInside().Count > 0;
    currentGameState = transition ? GameState.Transition : (inside ? GameState.Inside : GameState.Outside);
  }
}
