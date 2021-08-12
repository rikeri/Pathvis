using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DeleteAfterSound : MonoBehaviour
{
    AudioSource aus;
    // Start is called before the first frame update
    void Start()
    {
        aus = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!aus.isPlaying)
        Destroy(gameObject);
    }
}
