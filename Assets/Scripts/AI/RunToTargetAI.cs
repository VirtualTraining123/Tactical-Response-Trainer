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
    private State state = State.Running;

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
    
    protected void ToState(State newState) {
      state = newState;
      switch (state) {
        case State.Running:
          animator.SetTrigger(Run);
          animator.ResetTrigger(Crouch);
          animator.ResetTrigger(Shoot1);
          break;
        case State.Crouching:
          animator.ResetTrigger(Run);
          animator.SetTrigger(Crouch);
          animator.ResetTrigger(Shoot1);
          break;
        case State.Shooting:
          animator.ResetTrigger(Run);
          animator.ResetTrigger(Crouch);
          animator.SetTrigger(Shoot1);
          break;
        case State.Dead:
          enabled = false;
          break;
      }
    }

    private void Update() {
      switch (state) {
        case State.Running:
          UpdateRunning();
          break;
        case State.Crouching:
          UpdateCrouching();
          break;
        case State.Shooting:
          UpdateShooting();
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
    
    protected virtual void UpdateRunning() {
      if (!targetSpot) {
        targetSpot = markedLocations[Random.Range(0, markedLocations.Length)].transform;
      }

      if (HaveMadeItToTargetSpot()) {
        targetSpot = null;
        ToState(State.Crouching);
        navigationMesh.isStopped = true;
        return;
      }

      navigationMesh.isStopped = false;
      animator.enabled = true;
      ToState(State.Running);
      navigationMesh.SetDestination(targetSpot.position);
      // TODO: Play audio ???
    }

    protected virtual void UpdateCrouching() {
      ToState(State.Running);
    }
    
    protected virtual void UpdateShooting() {
      ToState(State.Running);

    }

    protected bool HaveMadeItToTargetSpot() {
      if (!targetSpot) {
        return false;
      }

      return (transform.position - targetSpot.position).sqrMagnitude <= 0.1f;
    }

    public void SetMarkedLocations(MarkedLocation[] markedLocations) {
      this.markedLocations = markedLocations;
    }
  }
}