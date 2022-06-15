using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class LaserPointerInteract : MonoBehaviour
{
    public SteamVR_Behaviour_Pose pose;
    public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");
    private RaycastHit hit;
    private LineRenderer line;
    private bool previousState = false;
    // private Button previousButton = null;
    // private Transform originalButtonTransform = null;
    // Start is called before the first frame update
    void Start()
    {
        line = this.GetComponent<LineRenderer>();
        line.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // do a raycast in the UI layer
        Ray raycast = new Ray(transform.position, transform.forward);
        bool didHit = Physics.Raycast(raycast, out hit, 10f, 1<<5);
        line.startColor = Color.white;
        line.endColor = Color.white;
        bool state = false;
        bool hitButton = false;
        Button button = null;

        if (didHit) 
        {   
            line.SetPositions(new Vector3[] {transform.position, hit.point});
            line.enabled = true;
            button = hit.collider.gameObject.GetComponent<Button>();
            if (button != null) {
                line.endColor = Color.green;
                hitButton = true;
                // previousButton = button;
            }
        } else {
            line.enabled = false;
        }

        if (interactWithUI.GetState(pose.inputSource)) state = true;

        // if (hitButton && !previousState && !state) {
        //     originalButtonTransform = button.transform;
        // }

        // if (hitButton && state && !previousState)
        // {
        //     button.transform.localPosition += new Vector3(0,0,6);
        // }

        if (previousState && !state && hitButton) 
        {
            button.onClick.Invoke();
            // button.transform.position = originalButtonTransform.position;
        }
        previousState = state;
    }
}
