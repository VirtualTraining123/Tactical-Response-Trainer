using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace AI {
  [RequireComponent(typeof(NavMeshAgent))]
  public class EnemyAI : RunToTargetAI {
    [Header("Crouch timing (ms)")]
    [SerializeField] public long minCrouchTimeMS = 5000;
    [SerializeField] public long maxCrouchTimeMS = 8000;

    [Header("Shooting settings")]
    [SerializeField] private float maxShootingDistance = 20f;
    [SerializeField] private float maxAimAngle = 60f;
    [SerializeField] private float minSpreadAngle = 0.5f;
    [SerializeField] private float maxSpreadAngle = 5f;
    [SerializeField] private float minAimTime = 0.5f;
    [SerializeField] private float maxAimTime = 1.2f;
    [SerializeField] private float bulletRadius = 0.05f;

    [SerializeField] private int minShotsToTake;
    [SerializeField] private int maxShotsToTake;
    [SerializeField] private float damage;
    [Range(0, 100)] [SerializeField] private float shootingAccuracy;
    [SerializeField] private Transform shootingPosition;

    private long timerMs;
    private long crouchDurationMs;

    private bool isAiming;
    private float aimTimer;

    private int currentShotsTaken;
    private int currentMaxShotsToTake;

    // Debug visualization storage
    private Vector3 lastVisionHitPoint;
    private bool hasLastVisionHit;
    private Vector3 lastShotHitPoint;
    private bool hasLastShotHit;

    protected override void Awake() {
      base.Awake();
      audioManager.Request("shot", gameObject);
    }

    protected override void OnDie() => Evaluator.OnEnemyKilled();

    protected override void UpdateCrouching() {
      if (CanSeePlayerViaSphereCast()) {
        Debug.Log("Player was visible, abort crouch");
        OnStartShooting();
        return;
      }

      var nowMs = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
      if (timerMs == 0) {
        timerMs = nowMs;
        // Random.Range returns int, cast to long
        crouchDurationMs = Random.Range((int)minCrouchTimeMS, (int)maxCrouchTimeMS);
        Debug.Log($"Entering Crouch: will crouch for {crouchDurationMs} ms");
        return;
      }

      var deltaMs = nowMs - timerMs;
      if (deltaMs <= crouchDurationMs) return;

      timerMs = 0;
      ToState(State.Running);
    }

    private void OnStartShooting() {
      currentShotsTaken = 0;
      currentMaxShotsToTake = Random.Range(minShotsToTake, maxShotsToTake);
      isAiming = true;
      aimTimer = Random.Range(minAimTime, maxAimTime);
      ToState(State.Shooting);
    }

    protected override void UpdateRunning() {
      if (CanSeePlayerViaSphereCast()) {
        OnStartShooting();
        return;
      }
      base.UpdateRunning();
    }

    protected override void UpdateShooting() {
      // Distance check
      float distance = Vector3.Distance(transform.position, Player.transform.position);
      if (distance > maxShootingDistance) {
        ToState(State.Running);
        return;
      }

      // Visibility check via SphereCast
      if (!CanSeePlayerViaSphereCast()) {
        ToState(State.Running);
        return;
      }

      // Field of view check
      Vector3 toTarget = (Player.transform.position - transform.position).normalized;
      float angle = Vector3.Angle(transform.forward, toTarget);
      if (angle > maxAimAngle) {
        RotateTowardsPlayer();
        return;
      }

      RotateTowardsPlayer();

      // Aiming delay
      if (isAiming) {
        aimTimer -= Time.deltaTime;
        if (aimTimer > 0f) return;
        isAiming = false;
      }

      // Continue shooting via animation events
      if (currentShotsTaken < currentMaxShotsToTake) return;

      ToState(State.Running);
    }

    public void Shoot() {
      RaycastShot(Player);
      currentShotsTaken++;
    }

    private bool CanSeePlayerViaSphereCast() {
      Vector3 origin = transform.position + Vector3.up * 1.6f; // eye height
      Vector3 targetPos = Player.GetBodyCenterPosition();
      Vector3 dir = (targetPos - origin).normalized;
      float distance = Vector3.Distance(origin, targetPos);

      // Debug draw cast line
      Debug.DrawLine(origin, origin + dir * Mathf.Min(distance, maxShootingDistance), Color.yellow, 0.1f);

      if (Physics.SphereCast(origin, bulletRadius, dir, out RaycastHit hit, maxShootingDistance)) {
        hasLastVisionHit = hit.collider.CompareTag("Player");
        lastVisionHitPoint = hit.point;
        return hasLastVisionHit;
      }

      hasLastVisionHit = false;
      lastVisionHitPoint = Vector3.zero;
      return false;
    }

    private void RaycastShot(Player player) {
      Vector3 origin = shootingPosition.position;
      Vector3 targetPos = player.GetBodyCenterPosition();
      float distance = Vector3.Distance(origin, targetPos);

      // Spread based on distance
      float normDist = Mathf.Clamp01(distance / maxShootingDistance);
      float spreadAngle = Mathf.Lerp(minSpreadAngle, maxSpreadAngle, normDist);
      Vector3 toPlayer = (targetPos - origin).normalized;
      Vector3 jitter = Random.insideUnitCircle * Mathf.Tan(spreadAngle * Mathf.Deg2Rad);
      Vector3 aimDir = (toPlayer + shootingPosition.right * jitter.x + shootingPosition.up * jitter.y).normalized;

      // Debug draw cast line
      Debug.DrawLine(origin, origin + aimDir * maxShootingDistance, Color.red, 0.1f);

      if (Physics.SphereCast(origin, bulletRadius, aimDir, out RaycastHit hit, maxShootingDistance)) {
        hasLastShotHit = true;
        lastShotHitPoint = hit.point;

        audioManager.Play("shot", gameObject);
        var hitObject = hit.collider.gameObject;

        if (!hit.collider.CompareTag("Player")) {
          Debug.Log("Ray hit non-player: " + hitObject.name);
          return;
        }

        if (Random.Range(0, 100) < shootingAccuracy) {
          Debug.Log("Shot the player!!!");
          player.TakeDamage(damage);
        } else {
          Debug.Log("But it missed :c");
        }
      } else {
        hasLastShotHit = false;
        lastShotHitPoint = Vector3.zero;
      }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
      if (!Application.isPlaying) return;
      if (hasLastVisionHit) {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(lastVisionHitPoint, bulletRadius);
      }
      if (hasLastShotHit) {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lastShotHitPoint, bulletRadius);
      }
    }
#endif
  }
}
