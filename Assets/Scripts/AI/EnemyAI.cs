using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace AI {
  [RequireComponent(typeof(NavMeshAgent))]
  public class EnemyAI : RunToTargetAI {
    [SerializeField] public long minCrouchTimeMS = 5000;
    [SerializeField] private int minShotsToTake;
    [SerializeField] private int maxShotsToTake;

    /// <summary>
    /// The damage the enemy deals to the player on each shot.
    /// </summary>
    [SerializeField] private float damage;

    /// <summary>
    /// The probability of the enemy hitting the player on each shot.
    /// </summary>
    [Range(0, 100)] [SerializeField] private float shootingAccuracy;

    protected override void Awake() {
      base.Awake();
      audioManager.Request("shot", gameObject);
    }

    /// <summary>
    /// The offset from the enemy's position to the position where the bullets are shot from.
    /// </summary>
    [SerializeField] private Transform shootingPosition;

    private long timerMs;

    private int currentShotsTaken;
    private int currentMaxShotsToTake;
    protected override void OnDie() => Evaluator.OnEnemyKilled();

    protected override void UpdateCrouching() {
      if (Player.IsVisibleFrom(transform.position)) {
        OnStartShooting();
        return;
      }

      var nowMs = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
      var deltaMs = nowMs - timerMs;
      if (deltaMs <= 0) {
        timerMs = nowMs;
        return;
      }

      if (deltaMs <= minCrouchTimeMS) return;
      ToState(State.Running);
      timerMs = 0;
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

      // Look at the player
      RotateTowardsPlayer();

      if (currentShotsTaken < currentMaxShotsToTake) return;
      ToState(State.Running);
    }


    // ReSharper disable once UnusedMember.Global This is called from the animation event.
    public void Shoot() {
      RaycastShot(Player);
      currentShotsTaken++;
    }

    private void RaycastShot(Player player) {
      if (!Physics.Linecast(shootingPosition.position, player.GetBodyCenterPosition(), out var hit)) return;
      Debug.DrawLine(shootingPosition.position, player.GetBodyCenterPosition(), Color.red, 1f);
      // Get the object that was hit

      audioManager.Play("shot", gameObject);
      var hitObject = hit.collider.gameObject;

      if (!hit.collider.CompareTag("Player")) {
        Debug.Log("Ray Hit something that is not the player, It hit: " + hitObject.name);
        return;
      }

      if (Random.Range(0, 100) < shootingAccuracy) {
        Debug.Log("Shot the player!!!");
        player.TakeDamage(damage);
      } else {
        Debug.Log("But it missed :c");
      }
    }
  }
}