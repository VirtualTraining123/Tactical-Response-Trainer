using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Animations {
  public class MovementAnimatorGroup : MovementAnimator {
    private enum AnimationEndTriggerType {
      All,
      First
    }
    [SerializeField] private List<MovementAnimator> animators;
    [SerializeField] private AnimationEndTriggerType triggerType = AnimationEndTriggerType.All;
    public override void SetActive(bool value) {
      animators.ForEach(x => x.SetActive(value));
    }

    public override void SetOnAnimationEnd(Action<Vector3> callback) {
      foreach (var animator in animators) {
        animator.SetOnAnimationEnd(OnAnimatorEnd);
      }
      OnAnimationEndCallbacks.Add(callback);
    }
    private void OnAnimatorEnd(Vector3 obj) {
      switch (triggerType) {
        case AnimationEndTriggerType.First: {
          foreach (var animator in animators) {
            animator.RemoveOnAnimationEnd(OnAnimatorEnd);
          }
          OnAnimationEndCallbacks.ForEach(x => x.Invoke(obj));
          OnAnimationEndCallbacks.Clear();
          break;
        }
        case AnimationEndTriggerType.All: {
          if (animators.All(x => !x.IsAnimating)) {
            OnAnimationEndCallbacks.ForEach(x => x.Invoke(obj));
            OnAnimationEndCallbacks.Clear();
          }
          break;
        }
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
