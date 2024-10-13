using System;
using UnityEngine;
using UnityEngine.AI;

namespace AI {
  [RequireComponent(typeof(NavMeshAgent))]
  public class CivilAI : RunToTargetAI {
    [SerializeField] public long minCrouchTimeMS = 5000;
    private long timerMs;
    protected override void OnDie() => Evaluator.OnCivilianKilled();

    protected override void UpdateCrouching() {
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
  }
}