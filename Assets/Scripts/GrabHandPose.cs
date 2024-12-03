using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

#if UNITY_EDITOR 
using UnityEditor;
#endif
public class GrabHandPose : MonoBehaviour
{
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
    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        
        grabInteractable.selectEntered.AddListener(SetupPose);
        grabInteractable.selectExited.AddListener(UnSetPose);
        rightHandPose.gameObject.SetActive(false);
        leftHandPose.gameObject.SetActive(false);
        
    }

  
    public void SetupPose(BaseInteractionEventArgs arg)
    {
        if (arg.interactorObject is XRDirectInteractor || arg.interactorObject is XRRayInteractor)
        {
            HandData handData = null;
            if (arg.interactorObject is XRBaseControllerInteractor controllerInteractor && controllerInteractor != null)
            {
                handData = controllerInteractor.xrController.transform.GetComponentInChildren<HandData>();
            }
            handData.animator.enabled = false;

            if (handData.handType == HandData.HandModelType.Right)
            {
                SetHandDataValues(handData, (handData.handType == HandData.HandModelType.Right) ? rightHandPose : leftHandPose, arg.interactorObject.transform);

            }
            else
            {
                SetHandDataValues(handData, (handData.handType == HandData.HandModelType.Right) ? rightHandPose : leftHandPose, arg.interactorObject.transform);

            }
            
            StartCoroutine(SetHandDataRoutine(handData,finalHandPosition, finalHandRotation , finalFingerRotations, startingHandPosition, startingHandRotation, startingFingerRotations));
           
        }
        
    }

    public void UnSetPose(BaseInteractionEventArgs arg)
    {
        if (arg.interactorObject is XRDirectInteractor || arg.interactorObject is XRRayInteractor)
        {
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.animator.enabled = true;
            
           
            StartCoroutine(SetHandDataRoutine(handData, startingHandPosition, startingHandRotation, startingFingerRotations,finalHandPosition, finalHandRotation , finalFingerRotations));
        }
    }
    public void SetHandDataValues(HandData h1, HandData h2, Transform interactorTransform)
    {
        startingHandPosition = h1.root.localPosition;
        finalHandPosition = h2.root.localPosition + (interactorTransform.localPosition / interactorTransform.localScale.x);  // Ajuste de posición
    
        startingHandRotation = h1.root.localRotation;
        finalHandRotation = h2.root.localRotation;
    
        startingFingerRotations = new Quaternion[h1.fingerBones.Length];
        finalFingerRotations = new Quaternion[h2.fingerBones.Length];

        for (int i = 0; i < h1.fingerBones.Length; i++)
        {
            startingFingerRotations[i] = h1.fingerBones[i].localRotation;
            finalFingerRotations[i] = h2.fingerBones[i].localRotation;
        }
    }


    public void SetHandData(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation)
    {
        h.root.localPosition = newPosition;
        h.root.localRotation = newRotation;

        for (int i = 0; i < newBonesRotation.Length; i++)
        {
            h.fingerBones[i].localRotation = newBonesRotation[i];
        }
    }

    public IEnumerator SetHandDataRoutine(HandData h, Vector3 newPosition, Quaternion newRotation, Quaternion[] newBonesRotation, Vector3 startingPosition, Quaternion startingRotation, Quaternion[] startingBonesRotation)
    {
        float timer = 0;

        while (timer < poseTransitionDuration)
        {
            Vector3 p = Vector3.Lerp(startingPosition, newPosition, timer / poseTransitionDuration);
            Quaternion r = Quaternion.Lerp(startingRotation, newRotation, timer / poseTransitionDuration);
            
            h.root.localPosition = p;
            h.root.localRotation = r;

            for (int i = 0; i < h.fingerBones.Length; i++)
            {
                h.fingerBones[i].localRotation = Quaternion.Lerp(startingBonesRotation[i], newBonesRotation[i], timer / poseTransitionDuration);
            }
            
            timer += Time.deltaTime;
            yield return null;
        }
    }
#if UNITY_EDITOR
    [MenuItem(("Tools, Mirror Selected Right Grab Pose"))]
    public static void MirrorRightPose()
    {
        Debug.Log("MirrorRightPose");
        GrabHandPose handPose = Selection.activeGameObject.GetComponent<GrabHandPose>();
        handPose.MirrorPose(handPose.leftHandPose, handPose.rightHandPose);
    }
    
#endif
    
    public void MirrorPose(HandData poseToMirror, HandData poseUsedToMirror)
    {
        Vector3 mirroredPosition = poseToMirror.root.localPosition;
        mirroredPosition.x *= -1;
        
        Quaternion mirroredQuaternion = poseUsedToMirror.root.localRotation;
        mirroredQuaternion.y *= -1;
        mirroredQuaternion.z *= -1;
        
        poseToMirror.root.localPosition = mirroredPosition;
        poseToMirror.root.localRotation = mirroredQuaternion;

        for (int i = 0; i < poseUsedToMirror.fingerBones.Length; i++)
        {
            poseToMirror.fingerBones[i].localRotation = poseUsedToMirror.fingerBones[i].localRotation;
        }
    }
}
