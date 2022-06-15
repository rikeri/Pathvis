using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class VRCameraFinder : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        GameObject vrcamera = GameObject.FindGameObjectWithTag("MainCamera");
        LookAtConstraint lookat = GetComponent<LookAtConstraint>();
        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = vrcamera.transform;
        source.weight = 1.0f;
        lookat.SetSource(0, source);
    }
}
