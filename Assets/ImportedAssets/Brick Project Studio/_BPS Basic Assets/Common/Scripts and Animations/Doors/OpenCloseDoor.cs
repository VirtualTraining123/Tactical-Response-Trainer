using UnityEditor;
using UnityEngine;

namespace SojaExiles {
  public class OpenCloseDoor : MonoBehaviour {
    public Animator animator;
    public Transform player;
    [SerializeField] public float openingDistance = 5f;
    [SerializeField] public float openingHeight = 5f;
    [SerializeField] public Vector3 openingTransform;

    private void FixedUpdate() {
      if (!player) return;
      if (IsInRange()) Closing();
      else Opening();
    }

    private bool IsInRange() {
      // return Vector3.Distance(player.position, openingTransform + transform.position) >= openingDistance;
      // Cylinder check
      var playerPos = new Vector2(player.position.x, player.position.z);
      var doorPos = new Vector2(transform.position.x, transform.position.z);
      var openingPos = new Vector2(openingTransform.x, openingTransform.z);
      var deltaR = Vector2.Distance(playerPos, doorPos + openingPos);
      var deltaH = Mathf.Abs(player.position.y - (transform.position.y + openingTransform.y));
      return deltaR <= openingDistance && deltaH <= openingHeight;
    }

    private void Opening() {
      animator.Play("Opening");
    }

    private void Closing() {
      animator.Play("Closing");
    }
    
    private void OnDrawGizmosSelected() {
      // Gizmos.color = Color.red;
    }
  }
  
  [CustomEditor(typeof(OpenCloseDoor))]
  public class OpenCloseDoorEditor : Editor {
    public void OnSceneGUI() {
      DrawDefaultInspector();
      var myScript = (OpenCloseDoor) target;
      
      EditorGUI.BeginChangeCheck();
      
      var newPosition = Handles.PositionHandle(myScript.transform.position + myScript.openingTransform, Quaternion.identity);
      
      if (EditorGUI.EndChangeCheck()) {
        Undo.RecordObject(myScript, "Change Look At Target Position");
        myScript.openingTransform = newPosition - myScript.transform.position;
      }
      
      EditorGUI.BeginChangeCheck();

      var newOpeningDistance = Handles.ScaleSlider(
        myScript.openingDistance,
        myScript.transform.position + myScript.openingTransform,
        Vector3.left,
        Quaternion.identity,
        1f,
        0f
      );
      var newOpeningHeight = Handles.ScaleSlider(
        myScript.openingHeight,
        myScript.transform.position + myScript.openingTransform,
        Vector3.up,
        Quaternion.identity,
        1f,
        0f
      );
      
      if (EditorGUI.EndChangeCheck()) {
        Undo.RecordObject(myScript, "Change Opening Distance");
        myScript.openingDistance = newOpeningDistance;
        myScript.openingHeight = newOpeningHeight;
      }
      
      // Draw the cylinder
      Handles.color = Color.red;
      Handles.DrawWireDisc(myScript.transform.position + myScript.openingTransform, Vector3.up, myScript.openingDistance);
      Handles.DrawWireDisc(myScript.transform.position + myScript.openingTransform + Vector3.up * myScript.openingHeight, Vector3.up, myScript.openingDistance);
      Handles.DrawLine(
        myScript.transform.position + myScript.openingTransform,
        myScript.transform.position + myScript.openingTransform + Vector3.up * myScript.openingHeight
      );
      
      
    }
  }
}