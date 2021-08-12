using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserInput : MonoBehaviour
{
    public static GameObject currentObject;
    int currentID;
    // Start is called before the first frame update
    void Start()
    {
        currentObject = null;
        currentID = 0;   
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, 10.0F);

        // go through and check if it was a button
        foreach(RaycastHit hit in hits) {
            int id = hit.collider.gameObject.GetInstanceID();

            if (currentID != id) {
                currentID = id;
                currentObject = hit.collider.gameObject;

                string name = currentObject.name;
            }
        }
    }
}
