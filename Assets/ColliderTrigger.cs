using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderTrigger : MonoBehaviour
{
    public Collider ofInterest;
    public UnityEvent OnInterestColliderEnter;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other) {
        if (other != ofInterest) return;
        OnInterestColliderEnter.Invoke();
    }
}
