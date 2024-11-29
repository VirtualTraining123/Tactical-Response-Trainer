using System;
using UnityEngine;

namespace SojaExiles {
  public class OpenCloseDoor : MonoBehaviour {
    public Animator animator;
    private void OnTriggerEnter(Collider collision) {
      if (collision.gameObject.CompareTag("MainCamera")) {
        animator.Play("Opening");
      }
    }
    
    private void OnTriggerExit(Collider collision) {
      if (collision.gameObject.CompareTag("MainCamera")) {
        animator.Play("Closing");
      }
    }
    
    // private void FixedUpdate() {
    //   if (!player) return;
    //   if (IsInRange()) Closing();
    //   else Opening();
    // }
    //
    // private bool IsInRange() {
    //   return openingCollider.bounds.Contains(player.position);
    // }
    //
    // private void Opening() {
    //   animator.Play("Opening");
    // }
    //
    // private void Closing() {
    //   animator.Play("Closing");
    // }
  }
}