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
    [SerializeField] protected Vector3? TargetPosition;
    private MarkedLocation[] markedLocations;

    protected override void Awake() {
      base.Awake();
      Player = FindFirstObjectByType<Player>(FindObjectsInactive.Include);
      if (!Player) {
        Debug.LogError("RunToTargetAI: Player not found in scene!");
      }
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
      TargetPosition ??= markedLocations[Random.Range(0, markedLocations.Length)].transform.position;

      if (HaveMadeItToTargetSpot()) {
        TargetPosition = null;
        ToState(State.Crouching);
        NavigationMesh.isStopped = true;
        return;
      }

      NavigationMesh.isStopped = false;
      Animator.enabled = true;
      ToState(State.Running);
      NavigationMesh.SetDestination(TargetPosition.Value);
    }

    private bool HaveMadeItToTargetSpot() {
      if (!TargetPosition.HasValue) return false;
      return (transform.position - TargetPosition.Value).sqrMagnitude <= 0.1f;
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
