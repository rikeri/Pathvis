using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSomeBall : MonoBehaviour
{
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        // transform.rotation *= Quaternion.Euler(0,1,0);
        transform.position += new Vector3(Random.Range(-1F, 1F), Random.Range(-1F, 1F), Random.Range(-1F, 1F))*0.01F;
        transform.rotation = Quaternion.Slerp(transform.rotation, Random.rotation, 0.03F);

    }
}
