using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wriggle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void recur(Transform mayHaveChildren) {
        // mayHaveChildren.rotation = Quaternion.Slerp(mayHaveChildren.rotation, Random.rotation, 0.03F);
        mayHaveChildren.localScale += new Vector3(Random.Range(-1F, 1F), Random.Range(-1F, 1F), Random.Range(-1F, 1F))*0.01F;
        mayHaveChildren.position += new Vector3(Random.Range(-1F, 1F), Random.Range(-1F, 1F), Random.Range(-1F, 1F))*0.01F;
        foreach(Transform child in mayHaveChildren) {
           recur(child);
        }
    }

    // Update is called once per frame
    void Update()
    {
        recur(transform);
        // foreach(Transform child in transform) {
        //     // transform.position += new Vector3(Random.Range(-1F, 1F), Random.Range(-1F, 1F), Random.Range(-1F, 1F))*0.01F;
        //     child.rotation = Quaternion.Slerp(child.rotation, Random.rotation, 0.03F);
        // }
    }
}
