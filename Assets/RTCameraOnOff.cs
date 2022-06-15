using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class RTCameraOnOff : MonoBehaviour
{
    public SteamVR_Action_Boolean fireAction;
    public Interactable interactable;
    public RasterPixels viewport;
    public AudioClip buttonSound;
    public TextMesh label;
    // Start is called before the first frame update

    public void UpdateLabel() {
        // resIdx = (resIdx + 1) % resCnt;
        // viewport.ChangeResolution(resolutions[resIdx,0], resolutions[resIdx,1], scales[resIdx]);
        // label.text = $"Resolution:\n{viewport.hPixels} x {viewport.vPixels}";
        StartCoroutine(LateLabel());
        AudioSource.PlayClipAtPoint(buttonSound, transform.position, 0.5f);
    }

    IEnumerator LateLabel() {
        yield return new WaitForSeconds(0.1f);
        label.text = viewport.cameraEnabled ? "ON" : "OFF";
    }
}
