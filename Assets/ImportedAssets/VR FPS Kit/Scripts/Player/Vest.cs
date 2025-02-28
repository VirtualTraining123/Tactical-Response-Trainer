using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vest : MonoBehaviour {
  private Transform head;

  [SerializeField] private float neckLength;

  // Start is called before the first frame update
  void Awake() {
    head = transform.parent.GetComponentInChildren<Camera>().transform;
  }

  // Update is called once per frame
  private void Update() {
    if (!head) {
      Destroy(gameObject);
      return;
    }

    var headAngle = Quaternion.RotateTowards(head.rotation, Quaternion.LookRotation(head.up), 30f);
    var desiredRotation = Quaternion.Euler(0, headAngle.eulerAngles.y, 0);
    var desiredPosition = head.transform.position - (head.up * neckLength);
    transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, 360f * Time.deltaTime);
    transform.position = desiredPosition;
  }

  private void OnDrawGizmosSelected() {
    var cam = transform.parent.GetComponentInChildren<Camera>();
    if (!cam) return;
    var headTransform = cam.transform;
    //Show the neck
    Gizmos.DrawLine(
      headTransform.transform.position,
      headTransform.transform.position - (headTransform.up * neckLength)
    );
  }
}