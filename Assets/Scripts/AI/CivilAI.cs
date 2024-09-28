using System;
using UnityEngine;
using UnityEngine.AI;

namespace AI {
  [RequireComponent(typeof(NavMeshAgent))]
  public class CivilAI : RunToTargetAI {
    private long timerMs;
    protected override void onDie() => Evaluator.OnCivilianKilled();

    protected override void UpdateCrouching() {
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
  }
}