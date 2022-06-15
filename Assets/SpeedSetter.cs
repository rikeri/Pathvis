using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSetter : MonoBehaviour
{
    public VisualTracer tracer;
    public float traceSpeed = 0.2f;
    public float mediumDelayTime = 0.5f;
    public float returnColorSpeed = 0.3f;
    public float finishedDelay = 0.1f;

    public void SetSpeed()
    {
        tracer.traceSpeed = traceSpeed;
        tracer.mediumDelayTime = mediumDelayTime;
        tracer.returnColorSpeed = returnColorSpeed;
        tracer.finishedDelay = finishedDelay;
    }
}
