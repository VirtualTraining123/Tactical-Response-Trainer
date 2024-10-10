using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace AI {
  [RequireComponent(typeof(NavMeshAgent))]
  public class EnemyAI : RunToTargetAI {
    [SerializeField] private int minShotsToTake;
    [SerializeField] private int maxShotsToTake;
    [SerializeField] protected float minTimeUnderCover;
    [SerializeField] protected float maxTimeUnderCover;

    /// <summary>
    /// The damage the enemy deals to the player on each shot.
    /// </summary>
    [SerializeField] private float damage;

    /// <summary>
    /// The probability of the enemy hitting the player on each shot.
    /// </summary>
    [Range(0, 100)] [SerializeField] private float shootingAccuracy;

    /// <summary>
    /// The offset from the enemy's position to the position where the bullets are shot from.
    /// </summary>
    [SerializeField] private Transform shootingPosition;

    private long timerMs;

    private int currentShotsTaken;
    private int currentMaxShotsToTake;
    protected override void onDie() => Evaluator.OnEnemyKilled();

    protected override void UpdateCrouching() {
      if (Player.IsVisibleFrom(transform.position)) {
        OnStartShooting();
        return;
      }
      var nowMs = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
      var deltaMs = nowMs - timerMs;
      if (timerMs == 0) {
        timerMs = nowMs;
        return;
      }

      switch (deltaMs) {
        case <= 0:
          timerMs = nowMs;
          break;
        case > 5000:
          ToState(State.Running);
          timerMs = 0;
          break;
      }
    }

    private void OnStartShooting() {
      currentShotsTaken = 0;
      currentMaxShotsToTake = Random.Range(minShotsToTake, maxShotsToTake);
      ToState(State.Shooting);
    }

    protected override void UpdateRunning() {
      if (Player.IsVisibleFrom(transform.position)) {
        OnStartShooting();
        return;
      }
      base.UpdateRunning();
    }

    protected override void UpdateShooting() {
      if (!Player.IsVisibleFrom(transform.position)) {
        ToState(State.Running);
        return;
      }

      if (currentShotsTaken < currentMaxShotsToTake) return;
      ToState(State.Running);
    }
    

    // ReSharper disable once UnusedMember.Global This is called from the animation event.
    public void Shoot() {
      var direction = Player.GetHeadPosition() - shootingPosition.position;
      RaycastShot(direction);
      currentShotsTaken++;
    }

    private void RaycastShot(Vector3 direction) {
      if (!Physics.Raycast(shootingPosition.position, direction, out var hit)) return;
      Debug.DrawRay(shootingPosition.position, direction, Color.green, 2.0f);
      var maybePlayer = hit.collider.GetComponentInParent<Player>();
      if (!maybePlayer) return;
      // TODO: Play audio?
      if (Random.Range(0, 100) < shootingAccuracy) maybePlayer.TakeDamage(damage);
    }
  }
}