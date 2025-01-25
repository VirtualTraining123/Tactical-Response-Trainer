using System.Collections.Generic;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Animations {
  public enum Hand {
    Left,
    Right,
  }

  public class HandsAnimationController : MonoBehaviour {
    private const string GRIP_AUX = "Grip_";
    private const string SELECT_AUX = "Trigger_";
    private const string PISTOL_AUX = "IsHoldingPistol_";
    private const string MAGAZINE_AUX = "IsHoldingMagazine_";
    [SerializeField] private Animator handAnimator;
    [SerializeField] private InputAction rightTriggerAction;
    [SerializeField] private InputAction leftTriggerAction;
    [SerializeField] private InputAction rightGrabAction;
    [SerializeField] private InputAction leftGrabAction;
    [SerializeField] private XRBaseInteractor leftInteractor;
    [SerializeField] private XRBaseInteractor rightInteractor;

    private static string For(string complement, Hand hand) {
      var s = hand.ToString();
      return complement + s[..1].ToUpper() + s[1..].ToLower();
    }

    private void Awake() {
      var hands = new Dictionary<XRBaseInteractor, Hand> {
        { rightInteractor, Hand.Right },
        { leftInteractor, Hand.Left }
      };

      foreach (var (interactor, hand) in hands) {
        interactor.selectEntered.AddListener(NewInteractionHandler(hand));
        interactor.selectExited.AddListener(_ => handAnimator.SetBool(For(PISTOL_AUX, hand), false));
      }
    }

    private UnityAction<SelectEnterEventArgs> NewInteractionHandler(Hand hand) {
      return arg => {
        var xrBaseInteractable = arg.interactableObject as XRBaseInteractable;
        if (xrBaseInteractable == null) {
          Debug.Log("No XRBaseInteractable found in grabbed item :c");
          return;
        }

        var o = xrBaseInteractable.gameObject;
        Debug.Log($"Grabbed object: {o.name} with tag: {o.tag}");
        switch (o.tag) {
          case "Weapon":
            handAnimator.SetBool(For(PISTOL_AUX, hand), true);
            break;
          case "Magazine":
            handAnimator.SetBool(For(MAGAZINE_AUX, hand), true);
            break;
          default:
            Debug.Log("No tag found in grabbed item :c");
            break;
        }
      };
    }

    private void Update() {
      handAnimator.SetFloat(For(SELECT_AUX, Hand.Right), rightTriggerAction.ReadValue<float>());
      handAnimator.SetFloat(For(SELECT_AUX, Hand.Left), leftTriggerAction.ReadValue<float>());
      handAnimator.SetFloat(For(GRIP_AUX, Hand.Right), rightGrabAction.ReadValue<float>());
      handAnimator.SetFloat(For(GRIP_AUX, Hand.Left), leftGrabAction.ReadValue<float>());
    }

    private void OnEnable() {
      rightTriggerAction.Enable();
      leftTriggerAction.Enable();
      rightGrabAction.Enable();
      leftGrabAction.Enable();
    }

    private void OnDisable() {
      rightTriggerAction.Disable();
      leftTriggerAction.Disable();
      rightGrabAction.Disable();
      leftGrabAction.Disable();
    }
  }
}