using System;
using System.Collections.Generic;
using UnityEngine;
namespace Animations {
  public class LinearMovementAnimator : MovementAnimator {
    [SerializeField] private Vector3 direction;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float distance = 5f;


    private bool active;
    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Start() {
      startPosition = GetPosition();
      endPosition = startPosition + direction * distance;
    }
    private Vector3 GetPosition() {
      return transform.localPosition;
    }

    private void SetPosition(Vector3 position) {
      transform.localPosition = position;
    }

    private void Update() {
      if (!IsAnimating) return;
      MoveTo(active ? endPosition : startPosition);
    }

    public override void SetActive(bool value) {
      active = value;
      IsAnimating = true;
    }

    private void MoveTo(Vector3 position) {
      var step = speed * Time.deltaTime * (Vector3.Distance(GetPosition(), position) * 0.5f + 0.1f);

      SetPosition(Vector3.MoveTowards(GetPosition(), position, step));
      if (!(Vector3.Distance(GetPosition(), position) < 0.001f)) return;
      SetPosition(position);
      EndAnimation();
    }
    private void EndAnimation() {
      IsAnimating = false;
      foreach (var callback in OnAnimationEndCallbacks) {
        callback.Invoke(GetPosition());
      }
      OnAnimationEndCallbacks.Clear();
    }
  }
}
