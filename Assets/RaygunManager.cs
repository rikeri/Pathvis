using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;

[RequireComponent( typeof( Interactable ) )]
public class RaygunManager : MonoBehaviour
{
    // Start is called before the first frame update
    public SteamVR_Action_Boolean fireAction;
    public Interactable interactable;
    public VisualTracer tracer;
    public GameObject colorDisplay;
    public GameObject ring_rays;    // gun is able to shoot rays
    public GameObject ring_normals; // normal vector is displayed at hit point
    public GameObject ring_scatter; // scatter direction is shown, letting paths be traced
    public GameObject ring_light;   // light information along the path is returned to the gun and displayed
    public AudioClip upgradeSound;
    public UnityEvent onUpgradeNormals;
    public UnityEvent onUpgradeScatter;
    public UnityEvent onUpgradeLight;
    private float attachTime = 0f;


    void Start()
    {
        ring_rays.GetComponent<MeshRenderer>().materials[1].color = Color.cyan;
    }

    public void EnableTooltips()
    {
        tracer.drawTooltips = true;
    }

    public void setBounceDepth(int count)
    {
        tracer.defaultBounceDepth = count;
        //AudioSource.PlayClipAtPoint(upgradeSound, transform.position, 0.5f);
    }

    public void EnableNormal()
    {
        if (!tracer.drawNormal)
        {
            ring_normals.GetComponent<MeshRenderer>().materials[1].color = Color.magenta;
            tracer.drawNormal = true;
            onUpgradeNormals.Invoke();
            AudioSource.PlayClipAtPoint(upgradeSound, transform.position, 0.5f);
        }
    }

    public void EnableScatter()
    {
        if (!tracer.drawScatter)
        {
            ring_scatter.GetComponent<MeshRenderer>().materials[1].color = Color.yellow;
            tracer.drawScatter = true;
            onUpgradeScatter.Invoke();
            AudioSource.PlayClipAtPoint(upgradeSound, transform.position, 0.5f);
        }
    }

    public void EnableLightReturn()
    {
        if (!tracer.doColorReturn)
        {
            ring_light.GetComponent<MeshRenderer>().materials[1].color = Color.white;
            tracer.doColorReturn = true;
            onUpgradeLight.Invoke();
            AudioSource.PlayClipAtPoint(upgradeSound, transform.position, 0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(interactable.attachedToHand != null) {
            SteamVR_Input_Sources source = interactable.attachedToHand.handType;

            if(fireAction[source].stateDown && Time.time - attachTime > 0.1f) {
                tracer.DoTrace();
            }
        }
    }

    private void OnAttachedToHand( Hand hand )
    {
        attachTime = Time.time;
    }

    public void UpdateColorDisplay() {
        colorDisplay.GetComponent<Renderer>().material.color = tracer.colorResult;
    }
}
