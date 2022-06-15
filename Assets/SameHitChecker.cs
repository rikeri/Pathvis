using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SameHitChecker : MonoBehaviour
{
    private bool trackingActive = false;
    private bool fulfilled = false;
    private Vector3 lastPos = Vector3.zero;
    private int closePositionCount = 0;
    public int closePositionCountGoal = 3;
    public float distanceThreshold = 0.1f;
    public VisualTracer tracer;
    public UnityEvent OnEnoughSameHits;
    // Start is called before the first frame update
    public void CheckHit()
    {   
        if (fulfilled) return;
        if (!trackingActive) return;
        if (!tracer.firstHitValid) return;

        if (Vector3.Distance(lastPos, tracer.firstHit.point) <= distanceThreshold) 
        {
            closePositionCount++;
        } else {
            closePositionCount = 0;
        }
        lastPos = tracer.firstHit.point;

        if (closePositionCount >= closePositionCountGoal) {
            OnEnoughSameHits.Invoke();
            fulfilled = true;
        }
    }

    public void EnableChecker() {
        trackingActive = true;
    }
}
