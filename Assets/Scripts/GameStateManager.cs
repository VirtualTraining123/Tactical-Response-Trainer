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
    CallbackWaiter<Vector3> waiter = new(end => {
      DisplayOutside();
      HideInside();
    });
    outsideAnimator?.SetOnAnimationEnd(waiter.CreateCallback());
    insideAnimator?.SetOnAnimationEnd(waiter.CreateCallback());
    transitionAnimator?.SetOnAnimationEnd(waiter.CreateCallback());
    outsideAnimator?.SetActive(true);
    insideAnimator?.SetActive(false);
    transitionAnimator?.SetActive(false);

  }

  public void ToTransition() {
    DisplayOutside();
    DisplayInside();
    HideIntersecting();
    outsideAnimator?.SetActive(false);
    insideAnimator?.SetActive(false);
    transitionAnimator?.SetActive(true);
  }

  public void ToInside() {
    CallbackWaiter<Vector3> waiter = new(end => {
      HideOutside();
      DisplayInside();
    });
    outsideAnimator?.SetOnAnimationEnd(waiter.CreateCallback());
    insideAnimator?.SetOnAnimationEnd(waiter.CreateCallback());
    transitionAnimator?.SetOnAnimationEnd(waiter.CreateCallback());

    outsideAnimator?.SetActive(false);
    insideAnimator?.SetActive(true);
    transitionAnimator?.SetActive(false);
  }
  public override void OnTransitionListener(AreaCollider.AreaCollider collider, Collider other) {
    var inside = InsiderCollider.GetPlayerInside().Count > 0;
    var transition = TransitionCollider.GetPlayerInside().Count > 0;
    currentGameState = transition ? GameState.Transition : (inside ? GameState.Inside : GameState.Outside);
  }
}
