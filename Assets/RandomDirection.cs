using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDirection : MonoBehaviour
{
    public VisualTracer tracer;
    public float directionSpread = 0.5f;
    public float spreadX = 0;
    public float spreadY = 0;
    public float spreadZ = 0;
    public BoxCollider boxBounds;
    

    // Update is called once per frame
    public void SetRandomDirectionAndTrace()
    {
        Vector3 target = tracer.transform.position + tracer.transform.forward + Random.insideUnitSphere*directionSpread; 
        tracer.SetDirection(target);
        tracer.DoTrace();
    }

    public void SetRandomOriginDirectionAndTrace()
    {
        if (boxBounds == null) {
            SetRandomDirectionAndTrace();
            return;
        }
        Vector3 bMin = boxBounds.bounds.min;
        Vector3 bMax = boxBounds.bounds.max;

        Vector3 worldOrigin = new Vector3(Random.Range(bMin.x, bMax.x),
                                          Random.Range(bMin.y, bMax.y),
                                          Random.Range(bMin.z, bMax.z));
        Vector3 target = tracer.transform.position + tracer.transform.forward + Random.insideUnitSphere*directionSpread; 
        // Vector3 worldOrigin = tracer.transform.position + new Vector3(Random.Range(-1, 1)*spreadX, Random.Range(-1, 1)*spreadY, Random.Range(-1, 1)*spreadZ);
        tracer.SetOrigin(worldOrigin);
        tracer.SetDirection(target);
        tracer.DoTrace();
    }
}
