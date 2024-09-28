using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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

    private int currentShotsTaken;
    private int currentMaxShotsToTake;

    private IEnumerator InitializeShootingCo() {
      yield return new WaitForSeconds(Random.Range(minTimeUnderCover, maxTimeUnderCover));
      StartShooting();
    }

    private void StartShooting() {
      currentMaxShotsToTake = Random.Range(minShotsToTake, maxShotsToTake);
      currentShotsTaken = 0;
      // animator.SetTrigger(Shoot1);
    }

    // ReSharper disable once UnusedMember.Global
    public void Shoot() {
      var direction = Player.GetHeadPosition() - shootingPosition.position;
      RaycastShot(direction);
      currentShotsTaken++;
      if (currentShotsTaken >= currentMaxShotsToTake) StartCoroutine(InitializeShootingCo());
    }

    private void RaycastShot(Vector3 direction) {
      if (!Physics.Raycast(shootingPosition.position, direction, out var hit)) return;
      Debug.DrawRay(shootingPosition.position, direction, Color.green, 2.0f);
      var maybePlayer = hit.collider.GetComponentInParent<Player>();
      if (!maybePlayer) return;
      // TODO: Play audio?
      if (Random.Range(0, 100) < shootingAccuracy) maybePlayer.TakeDamage(damage);
    }

    protected override void onDie() => Evaluator.OnEnemyKilled();
  }
}