using Audio;
using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace AI {
  [RequireComponent(typeof(NavMeshAgent))]
  public class CivilAI : RunToTargetAI {
    [SerializeField] public long minCrouchTimeMS = 5000;
    [SerializeField] public long maxCrouchTimeMS = 8000;

    private long timerMs = 0;
    private long crouchDurationMs;

    protected override void OnDie(BodyPart lastHitPart) {
      Evaluator.OnCivilianKilled(lastHitPart, name);
    }

    protected override bool ShouldRunWithGun() {
      return false;
    }
    protected override void UpdateCrouching() {
      var nowMs = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

      if (timerMs == 0) {
        timerMs = nowMs;
        crouchDurationMs = (long)Random.Range(minCrouchTimeMS, maxCrouchTimeMS);
        //Debug.Log($"Entering Crouch: will crouch for {crouchDurationMs} ms");
        return;
      }

      var deltaMs = nowMs - timerMs;
      if (deltaMs <= 0) {
        timerMs = nowMs;
        return;
      }
      if (deltaMs <= crouchDurationMs) return;

      ToState(State.Running);
      timerMs = 0;
    }
    protected override void UpdateShooting() {
      ToState(State.Running);
    }
  }
}
