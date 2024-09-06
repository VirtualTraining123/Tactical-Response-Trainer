using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabHandPose : MonoBehaviour
{
    public HandData rightHandPose;
    
    // Start is called before the first frame update
    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        
        grabInteractable.selectEntered.AddListener(SetupPose);
        
        rightHandPose.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    public void SetupPose(BaseInteractionEventArgs arg)
    {
        if (arg.interactorObject is XRDirectInteractor || arg.interactorObject is XRRayInteractor)
        {
            HandData handData = arg.interactorObject.transform.GetComponentInChildren<HandData>();
            handData.animator.enabled = false;
        }
    }
    
}
