using UnityEngine;
using System.Collections;


using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA.Input;
using UnityEngine.VR.WSA;

public class HolographicControl : MonoBehaviour {

    GestureRecognizer recognizer;

    // Use this for initialization
    void Start()
    {

    }

    // Use this for initialization
    void Awake()
    {

        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.TappedEvent += Recognizer_TappedEvent;

        recognizer.StartCapturingGestures();
    }

    private void Recognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        Debug.Log("tapped");

        // Figure out which hologram is focused this frame.
        GameObject focusedObject;

        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.SphereCast(headPosition,0.1f, gazeDirection, out hitInfo, 10.0f, 1 << 8) )
        {
            // If the raycast hit a hologram, use that as the focused object.
            focusedObject = hitInfo.collider.gameObject;
            Debug.Log("focusedObject is " + focusedObject.name);
            if( focusedObject.name == "AnchorSphere")
            {
                focusedObject.GetComponent<ARAnchor>().Place();
            }
        }
        else
        {
            // If the raycast did not hit a hologram, clear the focused object.
            focusedObject = null;
            Debug.Log("focusedObject is null");
        }
    }
}
