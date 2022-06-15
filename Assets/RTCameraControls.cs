using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class RTCameraControls : MonoBehaviour
{
    public SteamVR_Action_Boolean fireAction;
    public Interactable interactable;
    public RasterPixels viewport;
    public TextMesh label;
    public AudioClip buttonSound;
    int[,] resolutions = {
            {320, 240},
            {160, 120},
            {64, 48},
            {32, 24},
            {16, 12},
        };
    float[] scales = {
        0.004f,
        0.008f,
        0.02f,
        0.04f,
        0.08f,
    };
    int resIdx = 0;
    int resCnt = 5;

    // Start is called before the first frame update
    void Start()
    {
        label.text = $"Resolution:\n{viewport.hPixels} x {viewport.vPixels}";
    }

    public void CycleResolution() {
        resIdx = (resIdx + 1) % resCnt;
        viewport.ChangeResolution(resolutions[resIdx,0], resolutions[resIdx,1], scales[resIdx], true);
        label.text = $"Resolution:\n{viewport.hPixels} x {viewport.vPixels}";
        AudioSource.PlayClipAtPoint(buttonSound, transform.position, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        // if(interactable.attachedToHand != null) {
        //     SteamVR_Input_Sources source = interactable.attachedToHand.handType;

        //     if(fireAction[source].stateDown) {
                
        //     }
        // }
    }
}
