using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public Transform affectedObj;
    public float speed = 0.03f;

    private float val = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        affectedObj.position = Vector3.Lerp(pointA.position, pointB.position, (Mathf.Sin(val) + 1f)/2);
        val += speed;
    }
}
