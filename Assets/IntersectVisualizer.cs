using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectVisualizer : MonoBehaviour
{
    
    public ArrowRenderer normalArrow;
    public ArrowRenderer scatterArrow;
    public GameObject scatterSphere;
    private float scale = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Initialize(RaycastHit hit, RaytracingParticipator victim, Vector3 scatterDirection) {
        normalArrow.ArrowOrigin = hit.point;
        normalArrow.ArrowTarget = hit.point + scale*hit.normal;
        normalArrow.hasChanged = true;
        scatterArrow.ArrowOrigin = hit.point;
        switch(victim.materialType) {
            case RaytracingParticipator.MaterialType.Lambertian:
                GameObject lambSphere = Instantiate(scatterSphere, hit.point + scale/2*hit.normal, Quaternion.identity);
                lambSphere.transform.localScale = new Vector3(scale, scale, scale);
                lambSphere.transform.parent = transform;
                scatterArrow.ArrowTarget = Vector3.Lerp(hit.point, hit.point + scatterDirection, scale/2);
                break;
            case RaytracingParticipator.MaterialType.Mirror:
            default:
                scatterArrow.ArrowTarget = Vector3.Lerp(hit.point, hit.point + scatterDirection.normalized, scale);
                break;
        }
        scatterArrow.hasChanged = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
