using System;
using System.Collections.Generic;
using UnityEngine;
namespace Animations {
  public abstract class MovementAnimator : MonoBehaviour {
    protected readonly List<Action<Vector3>> OnAnimationEndCallbacks = new();
    public bool IsAnimating { get; protected set; }

    public virtual void SetOnAnimationEnd(Action<Vector3> callback) {
      OnAnimationEndCallbacks.Add(callback);
    }

    public virtual void RemoveOnAnimationEnd(Action<Vector3> callback) {
      OnAnimationEndCallbacks.Remove(callback);
    }

    public virtual void ClearOnAnimationEndCallbacks() {
      OnAnimationEndCallbacks.Clear();
    }

    public abstract void SetActive(bool value);
  }
}
