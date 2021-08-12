using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ShootVisualRay : MonoBehaviour
{
    public SteamVR_Action_Boolean fireAction;
    Vector3 rayOrigin = new Vector3(0,0,0);
    Vector3 rayDirection = new Vector3(0,0,1);

    private LineRenderer line; 
    public int maxBounceDepth = 6;
    public int defaultBounceDepth = 6;
    public float maxRayLength = 5f;
    public GameObject SoundViz;

    public Text outputLabel;
    private RaycastHit firstHit;
    private bool hitValid = false;
    public Interactable interactable;
    public Color background = Color.black;
    public GameObject colorviz;
    public GameObject intersectVisualizer;
    public GameObject colorBounceViz;
    public Transform skyVolumes;
    private float lineWidth = 0.008f;


    private List<GameObject> spheres = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        line = this.gameObject.GetComponent<LineRenderer>();
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = maxBounceDepth + 1;
    }

    IEnumerator BounceSound(float delay, Vector3 pos) {
        yield return new WaitForSeconds(delay);
        Instantiate(SoundViz, pos, Quaternion.identity);
    }
    
    void ShuutRey()
    {   
        // clear all the visualizers from the past path
        foreach (GameObject s in spheres) {
            Destroy(s);
        }

        Vector3 o = transform.TransformPoint(rayOrigin);
        Vector3 d = transform.TransformDirection(rayDirection);

        line.positionCount = maxBounceDepth + 2;
        line.SetPosition(0, o);

        background = Color.black;
        maxBounceDepth = defaultBounceDepth;
        foreach (Transform child in skyVolumes)
        {
            RTSkyVolume sky = child.gameObject.GetComponent<RTSkyVolume>();
            if (sky != null && sky.Contains(o)) {
                background = sky.skyColor;
                maxBounceDepth = sky.maxBounceDepth;
                break;
            }
        }
        
        // trace the path and display the resulting color on the light ball of the gun
        Color result = RayColor(o, d, maxBounceDepth);
        colorviz.GetComponent<Renderer>().material.color = result;
        
        if (hitValid)
            outputLabel.text = $"Bounces: {line.positionCount-2} (of {maxBounceDepth})"+
            "\nPos: " + firstHit.point.ToString("F3") + 
            "\nNorm: " + firstHit.normal.ToString("F3") +
            "\nUV: " + firstHit.textureCoord.ToString("F3") +
            "\nBaryc: " + firstHit.barycentricCoordinate.ToString("F3");
        else 
            outputLabel.text = "First ray didn't hit anything enabled for ray tracing.";
    }

    Color RayColor(Vector3 origin, Vector3 direction, int depth) {
        if (depth <= 0) {
            line.SetPosition(maxBounceDepth + 1, origin+direction*0.5f);
            return new Color(0f,0f,0f,1f);
        }
        int bounceIdx = maxBounceDepth - depth;
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, maxRayLength, 1<<8))
        {
            // check for participator
            RaytracingParticipator rp = hit.transform.gameObject.GetComponent<RaytracingParticipator>();
            if (rp == null) {
                return background;
            }
            if (bounceIdx == 0)
                firstHit = hit;
            hitValid = true;
            line.SetPosition(bounceIdx + 1, hit.point);

            // calc scatter direction to use for viz
            Vector3 scatterDir = rp.ScatterDirection(origin, hit);

            // create bounce visualizers
            GameObject isect = Instantiate(intersectVisualizer, hit.point, Quaternion.identity);
            GameObject isectColor = Instantiate(colorBounceViz, hit.point + hit.normal * 0.25f, Quaternion.identity);
            isectColor.transform.parent = isect.transform;
            IntersectVisualizer iv = isect.GetComponent<IntersectVisualizer>();
            iv.Initialize(hit, rp, scatterDir);
            spheres.Add(isect);
            
            // recursively trace the path and set the color of the visualizer ball
            Color result = rp.Emitted(hit) + rp.ScatterColor(hit) * RayColor(hit.point, scatterDir, depth-1);
            isectColor.GetComponent<Renderer>().material.color = result;
            return result;

        } else{
            // add a sky "hit" visualizer
            line.SetPosition(bounceIdx + 1, origin+direction*3.5f);

            GameObject isectColor = Instantiate(colorBounceViz, origin+direction*3.5f, Quaternion.identity);
            isectColor.GetComponent<Renderer>().material.color = background;
            spheres.Add(isectColor);

            line.positionCount = bounceIdx + 2;
            return background;
        }
    }


    // Update is called once per frame
    void Update() {
        if(interactable.attachedToHand != null) {
            SteamVR_Input_Sources source = interactable.attachedToHand.handType;

            if(fireAction[source].stateDown) {
                ShuutRey();
            }
        }
    }
}
