using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlaceInTriggerGoal : MonoBehaviour
{
    public UnityEvent OnSuccess;
    public BoxCollider triggerArea; // area this objects needs to be in to count as a success
    public void CheckIfInside()
    {
        if (triggerArea.bounds.Contains(transform.position)) OnSuccess.Invoke();
    }
}
