using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace HandPose {
  public class GrabHandPose : MonoBehaviour {
    public float poseTransitionDuration = 0.2f;
    public HandData rightHandPose;
    public HandData leftHandPose;
    private Vector3 startingHandPosition;
    private Vector3 finalHandPosition;
    private Quaternion startingHandRotation;
    private Quaternion finalHandRotation;

    private Quaternion[] startingFingerRotations;
    private Quaternion[] finalFingerRotations;

    // Start is called before the first frame update
    private void Start() {
      var grabInteractable =
        GetComponent<XRGrabInteractable>();

      grabInteractable.selectEntered.AddListener(SetupPose);
      grabInteractable.selectExited.AddListener(UnSetPose);
      rightHandPose.gameObject.SetActive(false);
      leftHandPose.gameObject.SetActive(false);
    }


    private void SetupPose(BaseInteractionEventArgs arg) {
      if (arg.interactorObject is not XRDirectInteractor &&
          arg.interactorObject is not XRRayInteractor) return;
      var handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
      handData.animator.enabled = false;

      SetHandDataValues(handData, handData.handType == HandData.HandModelType.Right ? rightHandPose : leftHandPose);

      StartCoroutine(
        SetHandDataRoutine(
          handData,
          finalHandPosition,
          finalHandRotation,
          finalFingerRotations,
          startingHandPosition,
          startingHandRotation,
          startingFingerRotations
        )
      );
    }

    private void UnSetPose(BaseInteractionEventArgs arg) {
      if (arg.interactorObject is not XRDirectInteractor && arg.interactorObject is not XRRayInteractor) return;
      var handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
      handData.animator.enabled = true;


      StartCoroutine(
        SetHandDataRoutine(
          handData,
          startingHandPosition,
          startingHandRotation,
          startingFingerRotations,
          finalHandPosition,
          finalHandRotation,
          finalFingerRotations
        )
      );
    }

    private void SetHandDataValues(HandData h1, HandData h2) {
      startingHandPosition = new Vector3(
        h1.root.localPosition.x / h1.root.localScale.x,
        h1.root.localPosition.y / h1.root.localScale.y,
        h1.root.localPosition.z / h1.root.localScale.z
      );
      finalHandPosition = new Vector3(
        h2.root.localPosition.x / h2.root.localScale.x,
        h2.root.localPosition.y / h2.root.localScale.y,
        h2.root.localPosition.z / h2.root.localScale.z
      );

      startingHandRotation = h1.root.localRotation;
      finalHandRotation = h2.root.localRotation;

      startingFingerRotations = new Quaternion[h1.fingerBones.Length];
      finalFingerRotations = new Quaternion[h2.fingerBones.Length];

      for (var i = 0; i < h1.fingerBones.Length; i++) {
        startingFingerRotations[i] = h1.fingerBones[i].localRotation;
        finalFingerRotations[i] = h2.fingerBones[i].localRotation;
      }
    }

    public void SetHandData(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation) {
      h.root.localPosition = newPosition;
      h.root.localRotation = newRotation;

      for (var i = 0; i < newBonesRotation.Length; i++) {
        h.fingerBones[i].localRotation = newBonesRotation[i];
      }
    }

    private IEnumerator SetHandDataRoutine(HandData h, Vector3 newPosition, Quaternion newRotation,
      Quaternion[] newBonesRotation, Vector3 startingPosition, Quaternion startingRotation,
      Quaternion[] startingBonesRotation) {
      float timer = 0;

      while (timer < poseTransitionDuration) {
        var p = Vector3.Lerp(startingPosition, newPosition, timer / poseTransitionDuration);
        var r = Quaternion.Lerp(startingRotation, newRotation, timer / poseTransitionDuration);

        h.root.localPosition = p;
        h.root.localRotation = r;

        for (var i = 0; i < h.fingerBones.Length; i++) {
          h.fingerBones[i].localRotation = Quaternion.Lerp(
            startingBonesRotation[i],
            newBonesRotation[i],
            timer / poseTransitionDuration
          );
        }

        timer += Time.deltaTime;
        yield return null;
      }
    }
#if UNITY_EDITOR
    // [MenuItem(("Tools, Mirror Selected Right Grab Pose"))]
    // public static void MirrorRightPose() {
    //   Debug.Log("MirrorRightPose");
    //   var handPose = Selection.activeGameObject.GetComponent<GrabHandPose>();
    //   MirrorPose(handPose.leftHandPose, handPose.rightHandPose);
    // }

#endif

    private static void MirrorPose(HandData poseToMirror, HandData poseUsedToMirror) {
      var mirroredPosition = poseToMirror.root.localPosition;
      mirroredPosition.x *= -1;

      var mirroredQuaternion = poseUsedToMirror.root.localRotation;
      mirroredQuaternion.y *= -1;
      mirroredQuaternion.z *= -1;

      poseToMirror.root.localPosition = mirroredPosition;
      poseToMirror.root.localRotation = mirroredQuaternion;

      for (var i = 0; i < poseUsedToMirror.fingerBones.Length; i++) {
        poseToMirror.fingerBones[i].localRotation = poseUsedToMirror.fingerBones[i].localRotation;
      }
    }
  }
}