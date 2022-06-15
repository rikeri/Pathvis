using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightHitChecker : MonoBehaviour
{
    private bool trackingActive = false;
    private bool fulfilled = false;
    private int lightHitCount = 0;
    public int lightHitCountGoal = 3;
    public VisualTracer tracer;
    public UnityEvent OnEnoughLightPaths;
    // Start is called before the first frame update
    public void CheckLight()
    {   
        if (fulfilled) return;
        if (!trackingActive) return;
        if (tracer.colorResult.maxColorComponent > 0.1f) lightHitCount++;
        if (lightHitCount >= lightHitCountGoal) {
            OnEnoughLightPaths.Invoke();
            fulfilled = true;
        }
    }

    public void EnableChecker() {
        trackingActive = true;
    }
}
