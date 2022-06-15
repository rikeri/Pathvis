using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGetter : MonoBehaviour
{
    public GameObject objectToGet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetObject()
    {
        // make sure object is not attached to hand
        if (objectToGet.transform.parent == null) {
            // objectToGet.transform.SetPositionAndRotation(transform.position, transform.rotation);
            StopAllCoroutines();
            StartCoroutine(SmoothTransition());
        }
    }

    IEnumerator SmoothTransition()
    {
        Transform start = objectToGet.transform;
        float moveTime = 0.1f;
        for (float t = 0; t < moveTime; t+= Time.deltaTime)
        {
            objectToGet.transform.position = Vector3.Lerp(start.position, transform.position, t/moveTime);
            objectToGet.transform.rotation = Quaternion.Lerp(start.rotation, transform.rotation, t/moveTime);
            yield return null;
        }
        objectToGet.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }
}
