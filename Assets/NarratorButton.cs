using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarratorButton : MonoBehaviour
{
    public AudioSource source;
    public AudioClip clip;
    private Material buttonMaterial;
    public NarrationManager manager;
    
    // Start is called before the first frame update
    void Start()
    {
        buttonMaterial = this.GetComponent<Renderer>().material;
        source.clip = clip;
    }

    public void Pressed()
    {
        if (source.isPlaying)
        {
            StopNarration();
        } else {
            manager.StartedPlaying(this);
            source.Play();
            buttonMaterial.color = Color.green;
            Invoke("StopNarration", clip.length);
        }
    }

    public void StopNarration()
    {
        CancelInvoke();
        source.Stop();
        buttonMaterial.color = Color.white;
    }
}
