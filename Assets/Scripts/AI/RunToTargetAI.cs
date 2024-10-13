using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI {
  public enum State {
    Running,
    Crouching,
    Shooting,
    Dead
  }
  public abstract class RunToTargetAI : AI {
    [SerializeField] public float rotationSpeed;
    protected Player Player;
    [SerializeField] private Transform targetSpot;
    private MarkedLocation[] markedLocations;

    protected override void Awake() {
      base.Awake();
      Player = FindObjectOfType<Player>();
    }

    protected void RotateTowardsPlayer() {
      var direction = Player.GetHeadPosition() - transform.position;
      direction.y = 0;
      transform.rotation = Quaternion.RotateTowards(
        transform.rotation,
        Quaternion.LookRotation(direction),
        rotationSpeed * Time.deltaTime
      );
    }

    protected override void UpdateRunning() {
      if (!targetSpot) {
        targetSpot = markedLocations[Random.Range(0, markedLocations.Length)].transform;
      }

      if (HaveMadeItToTargetSpot()) {
        targetSpot = null;
        ToState(State.Crouching);
        NavigationMesh.isStopped = true;
        return;
      }

      NavigationMesh.isStopped = false;
      Animator.enabled = true;
      ToState(State.Running);
      NavigationMesh.SetDestination(targetSpot.position);
      // TODO: Play audio ???
    }

    protected override void UpdateCrouching() {
      ToState(State.Running);
    }
    
    protected override void UpdateShooting() {
      ToState(State.Running);

    }

    private bool HaveMadeItToTargetSpot() {
      if (!targetSpot) return false;
      return (transform.position - targetSpot.position).sqrMagnitude <= 0.1f;
    }

    public void SetMarkedLocations(MarkedLocation[] markedLocations) {
      this.markedLocations = markedLocations;
    }

    protected override void UpdateDead() {
      NavigationMesh.isStopped = true;
      Animator.enabled = false;
    }
  }
}