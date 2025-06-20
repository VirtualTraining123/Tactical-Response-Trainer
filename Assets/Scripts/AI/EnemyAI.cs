using Audio;
using System;
using Unity.VisualScripting;
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
    [SerializeField] private GameObject headTransform;

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
    

    protected override void OnDie(BodyPart finalHitPart) {
      Evaluator.OnEnemyKilled(finalHitPart, name + GetInstanceID());
    }

    protected override void UpdateCrouching() {
      if (IsPlayerVisible()) {
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

    protected override bool ShouldRunWithGun() {
      return true;
    }
    protected override void UpdateRunning() {
      if (IsPlayerVisible()) {
        TargetPosition = null;
        NavigationMesh.isStopped = true;
        OnStartShooting();
      } else {
        base.UpdateRunning();
      }
    }

    protected override void UpdateShooting() {
      // Distance check
      var distance = Vector3.Distance(transform.position, Player.transform.position);
      if (distance > maxShootingDistance) {
        ToState(State.Running);
        return;
      }

      // Visibility check via SphereCast
      if (!IsPlayerVisible()) {
        ToState(State.Running);
        TargetPosition = new(
          lastVisionHitPoint.x,
          transform.position.y,
          lastVisionHitPoint.z
        );
        return;
      }

      // Field of view check
      var toTarget = (Player.transform.position - transform.position).normalized;
      var angle = Vector3.Angle(transform.forward, toTarget);
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

    // Called by animation event (Shoot animation)
    public void AnimationShoot() {
      RaycastShot(Player);
      currentShotsTaken++;
    }

    private bool IsPlayerVisible() {
      var origin = headTransform.transform.position;
      var targetPos = Player.GetBodyCenterPosition();
      var dir = (targetPos - origin).normalized;
      var distance = Vector3.Distance(origin, targetPos);

      // Debug draw cast line
      Debug.DrawLine(origin + dir * 0.5f, origin + dir * Mathf.Min(distance, maxShootingDistance), Color.yellow, 0.1f);

      if (Physics.SphereCast(origin + dir * 0.5f, bulletRadius, dir, out var hit, maxShootingDistance, ~LayerMask.GetMask("Enemy"))) {
        // Debug draw cast line
        Debug.DrawLine(origin, hit.point, Color.cyan, 0.1f);
        hasLastVisionHit = IsPlayer(hit);
        lastVisionHitPoint = hit.point;
        return hasLastVisionHit;
      }

      hasLastVisionHit = false;
      lastVisionHitPoint = Vector3.zero;
      return false;
    }

    private void RaycastShot(Player player) {
      var origin = shootingPosition.position;
      var targetPos = player.GetBodyCenterPosition();
      var distance = Vector3.Distance(origin, targetPos);

      // Spread based on distance
      var normDist = Mathf.Clamp01(distance / maxShootingDistance);
      var spreadAngle = Mathf.Lerp(minSpreadAngle, maxSpreadAngle, normDist);
      var toPlayer = (targetPos - origin).normalized;
      Vector3 jitter = Random.insideUnitCircle * Mathf.Tan(spreadAngle * Mathf.Deg2Rad);
      var aimDir = (toPlayer + shootingPosition.right * jitter.x + shootingPosition.up * jitter.y).normalized;

      // Debug draw cast line
      Debug.DrawLine(origin, origin + aimDir * maxShootingDistance, Color.red, 0.1f);

      if (!Physics.SphereCast(origin, bulletRadius, aimDir, out var hit, maxShootingDistance, ~LayerMask.GetMask("Enemy"))) {
        hasLastShotHit = false;
        lastShotHitPoint = Vector3.zero;
        return;
      }
      Debug.DrawLine(origin, hit.point, Color.magenta, 0.1f);
      hasLastShotHit = true;
      lastShotHitPoint = hit.point;

      InlineAudioManager.GetAudioSource(ClipName.GUNSHOT).Play();
      var hitObject = hit.collider.gameObject;

      if (!IsPlayer(hit)) {
        Debug.Log("Ray hit non-player: " + hitObject.name);
        return;
      }

      if (Random.Range(0, 100) < shootingAccuracy) {
        Debug.Log("Shot the player!!!");
        player.TakeDamage(damage);
      } else {
        Debug.Log("But it missed :c");
      }
    }
    private static bool IsPlayer(RaycastHit hit) {
      var output = hit.collider.CompareTag("Player") || hit.collider.CompareTag("LeftHand") || hit.collider.CompareTag("RightHand");
      if (!output) {
        Debug.Log($"Raycast hit non-player: {hit.collider.name} (Tag: {hit.collider.tag})");
      }
      return output;
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
      Gizmos.color = IsPlayerVisible() ? Color.green: Color.red;
      Gizmos.DrawWireSphere(transform.position + Vector3.up * 2, bulletRadius);
    }
#endif
  }
}
