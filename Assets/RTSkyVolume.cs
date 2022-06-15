using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class RTSkyVolume : MonoBehaviour
{
    public Color skyColor = Color.black;
    public int maxBounceDepth = 6;
    private BoxCollider coll;

    // Start is called before the first frame update
    void Start()
    {
        if (coll == null)
            coll = this.GetComponent<BoxCollider>();
    }
    public bool Contains(Vector3 point) {
        if (coll == null)
            coll = this.GetComponent<BoxCollider>();
        return coll.bounds.Contains(point);
    }
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
