using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundsRenderer : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject bboxViz;

    GameObject CreateBBox(Vector3 min, Vector3 max) {
        Vector3 center = Vector3.Lerp(min, max, 0.5f);
        GameObject gobj = Instantiate(bboxViz, center, Quaternion.identity);
        gobj.transform.localScale = max - min;
        return gobj;
    }

    void Start()
    {
        Vector3 min;
        Vector3 max;
        // go through all child objects with renderers and add up their bounds
        Renderer firstch = GetComponentInChildren(typeof(Renderer)) as Renderer;
        if (firstch != null) {
            min = firstch.bounds.min;
            max = firstch.bounds.max;
        } else {
            return;
        }

        Component[] rs;
        rs = GetComponentsInChildren(typeof(Renderer));

        if(rs != null) {
            foreach(Renderer r in rs)
            {
                min = Vector3.Min(min, r.bounds.min);
                max = Vector3.Max(max, r.bounds.max);
                CreateBBox(r.bounds.min, r.bounds.max);
            }
        }
        CreateBBox(min, max);
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
