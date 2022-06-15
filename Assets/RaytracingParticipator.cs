using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaytracingParticipator : MonoBehaviour
{
    public enum MaterialType {
        Lambertian,
        Mirror,
        Refractive,
        Lamp,
        RayBlocker
    };

    public MaterialType materialType;
    public float MirrorRoughness = 0f;
    public float LampStrength = 5f;
    public Color LampColor = Color.white;
    public float IOR = 1.4f;
    public UnityEvent onHit;
    public bool barrier = false; // barriers won't have tooltips or visualizers shown

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private Vector3 refract(Vector3 incident, Vector3 norm, float ior) {
        float refract_ratio = 1 / ior;
        float cos_theta = Vector3.Dot(-incident, norm);
        Vector3 r_out_perp = refract_ratio * (incident + cos_theta * norm);
        Vector3 r_out_parallel = -Mathf.Sqrt(Mathf.Abs(1f - r_out_perp.sqrMagnitude)) * norm;
        return r_out_perp + r_out_parallel;
    }

    public Vector3 ScatterDirection(Vector3 origin, RaycastHit hit) {
        switch(materialType) {
            case MaterialType.Mirror:
                Vector3 reflected = Vector3.Reflect((hit.point - origin).normalized, hit.normal);
                
                return reflected + MirrorRoughness * Random.insideUnitSphere;
            case MaterialType.Refractive:
                Vector3 refracted = refract((hit.point - origin).normalized, hit.normal, IOR);

                return refracted + MirrorRoughness * Random.insideUnitSphere; 
            case MaterialType.Lambertian:
            default:
                return Random.onUnitSphere + hit.normal;

        }
    }

    public Color ScatterColor(RaycastHit hit) {
        Vector2 texcoord = new Vector2(0f,0f);
        Color col = new Color(0f,0f,0f,1f);
        switch(materialType) {
            case MaterialType.Lamp:
                return Color.black;
                // return new Color(1f, 1f, 1f, 1f);
            case MaterialType.Refractive:
                return new Color(0.9f, 0.9f, 0.9f, 1f);
            case MaterialType.Mirror:
            case MaterialType.Lambertian:
            default:
                texcoord = hit.textureCoord;
                Renderer r = hit.transform.gameObject.GetComponent<Renderer>(); // sample color from material
                if (r != null) {
                    col = r.material.color;
                    if (r.material.mainTexture != null) {
                        Texture2D texas = (r.material.mainTexture as Texture2D);
                        if (texas.isReadable)
                        col *= texas.GetPixelBilinear(hit.textureCoord[0], hit.textureCoord[1]);
                    }
                }
                break;

        }
        return col;
    }

    public Color Emitted(RaycastHit hit) {
        switch(materialType) {
            case MaterialType.Lamp:
                return LampColor * LampStrength;
            default:
                return new Color(0f,0f,0f,1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
