using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveTrackerDemo : MonoBehaviour
{
    public VisualTracer tracer;
    private BoxCollider box;
    public Text goalText;
    private bool goal1lambert = false;
    private bool goal2metal = false;
    private bool goal3glass = false;
    public AudioClip goalSound;
    public Transform goalSoundPlacement;
    // Start is called before the first frame update
    void Start()
    {
        box = this.GetComponent<BoxCollider>();
        tracer.onTraceVisualDone.AddListener(GoalUpdate);
        SetNewText();
    }

    void GoalUpdate()
    {
        bool n_lambert = false;
        bool n_mirror = false;
        bool n_refract = false;
        bool playsound = false;
        // check if tracer is within the bounds for this area
        if (box.bounds.Contains(tracer.transform.position))
        {
            SegmentInfo firstSeg = tracer.segments[0].GetComponent<SegmentInfo>();
            if (firstSeg.segType == SegmentInfo.SegmentType.Hit)
            {
                switch (firstSeg.participator.materialType)
                {
                    case RaytracingParticipator.MaterialType.Lambertian:
                        n_lambert = true;
                        break;
                    case RaytracingParticipator.MaterialType.Mirror:
                        n_mirror = true;
                        break;
                    case RaytracingParticipator.MaterialType.Refractive:
                        n_refract = true;
                        break;
                    default:
                        break;
                }
            }
        }
        if (n_lambert && !goal1lambert) playsound = true;
        if (n_mirror && !goal2metal) playsound = true;
        if (n_refract && !goal3glass) playsound = true;
        goal1lambert |= n_lambert;
        goal2metal |= n_mirror;
        goal3glass |= n_refract;
        if (playsound) {
            AudioSource.PlayClipAtPoint(goalSound, goalSoundPlacement.position);
            SetNewText();
        }
    }

    void SetNewText()
    {
        string result = "Gameplay goal demo\n\n" +
        $"[{(goal1lambert?'X':' ')}]  hit a lambertian material\n" +
        $"[{(goal2metal?'X':' ')}]  hit a reflective material\n" +
        $"[{(goal3glass?'X':' ')}]  hit a refractive material\n";
        goalText.text = result;
    }
}
