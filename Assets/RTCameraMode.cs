using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class RTCameraMode : MonoBehaviour
{
    // public SteamVR_Action_Boolean fireAction;
    // public Interactable interactable;
    public RasterPixels viewport;
    public TextMesh label;
    public AudioClip buttonSound;
    RenderType[] modes = {
            RenderType.PathTracing,
            RenderType.SimpleRT,
            RenderType.Normals,
            RenderType.UVs
        };
    int modeIdx = 0;
    int modeCnt = 3; // UVs mode is skipped - texture coordinates are not explained

    void Start() {
        label.text = $"mode:\n{viewport.renderMode}";
    }

    public void CycleModes() {
        modeIdx = (modeIdx + 1) % modeCnt;
        // switch (modes[modeIdx]) {
        //     case RenderType.PathTracing:
        //         viewport.SetRenderPT();
        //         break;
        //     case RenderType.SimpleRT:
        //         viewport.SetRenderSimpleRT();
        //         break;
        // }
        viewport.SetRenderMode(modes[modeIdx]);
        label.text = $"mode:\n{viewport.renderMode}";
        AudioSource.PlayClipAtPoint(buttonSound, transform.position, 0.5f);
    }

}
