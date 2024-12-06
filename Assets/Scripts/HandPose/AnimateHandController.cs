using UnityEngine;
using UnityEngine.InputSystem;

namespace HandPose {
  [RequireComponent(typeof(Animator))]
  public class AnimateHandController : MonoBehaviour {
    private static readonly int Grip = Animator.StringToHash("Grip");
    private static readonly int Trigger = Animator.StringToHash("Trigger");
    public InputActionReference gripInputActionReference;
    public InputActionReference triggerInputActionReference;

    private Animator handAnimator;
    private float gripValue;
    private float triggerValue;

    public void Start() {
      handAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void Update() {
      AnimateGrip();
      AnimateTrigger();
    }

    private void AnimateGrip() {
      gripValue = gripInputActionReference.action.ReadValue<float>();
      handAnimator.SetFloat(Grip, gripValue);
    }

    private void AnimateTrigger() {
      triggerValue = triggerInputActionReference.action.ReadValue<float>();
      handAnimator.SetFloat(Trigger, triggerValue);
    }
  }
}