using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NormalDirectionChecker : MonoBehaviour
{
    private bool trackingActive = false;
    private bool fulfilled = false;
    public VisualTracer tracer;
    public UnityEvent OnVerticalNormal;
    // Start is called before the first frame update
    public void CheckNormal()
    {   
        if (fulfilled) return;
        if (!trackingActive) return;
        if (!tracer.firstHitValid) return;
        float dotprod = Vector3.Dot(tracer.firstHit.normal, Vector3.down);
        if (dotprod >= 0.99) {
            OnVerticalNormal.Invoke();
            fulfilled = true;
        }
    }

    public void EnableChecker() {
        trackingActive = true;
    }

}
