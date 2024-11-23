using UnityEngine;

namespace SojaExiles {
  public class OpenCloseDoor : MonoBehaviour {
    public Animator animator;
    public Transform player;
    [SerializeField] private float openingDistance = 5f;
    [SerializeField] private Vector3 openingTransform;

    private void FixedUpdate() {
      if (!player) return;
      if (Vector3.Distance(player.position, openingTransform + transform.position) >= openingDistance) Closing();
      else Opening();
    }

    private void Opening() {
      animator.Play("Opening");
    }

    private void Closing() {
      animator.Play("Closing");
    }
    
    // On gyzmo show sphere
    private void OnDrawGizmosSelected() {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position + openingTransform, openingDistance);
      Gizmos.DrawWireSphere(transform.position + openingTransform, 0.1f);
    }
  }
}