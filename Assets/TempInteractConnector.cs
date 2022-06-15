using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class TempInteractConnector : MonoBehaviour
{
    public SteamVR_Action_Boolean fireAction;
    public Interactable interactable;
    private VisualTracer tracer;
    public GameObject colorBall;

    // Start is called before the first frame update
    void Start()
    {
        tracer = this.GetComponent<VisualTracer>();   
    }

    // Update is called once per frame
    void Update()
    {
        if(interactable.attachedToHand != null) {
            SteamVR_Input_Sources source = interactable.attachedToHand.handType;

            if(fireAction[source].stateDown) {
                tracer.DoTrace();
            }
        }
    }

    public void UpdateColorBall() {
        colorBall.GetComponent<Renderer>().material.color = tracer.colorResult;
    }
}
