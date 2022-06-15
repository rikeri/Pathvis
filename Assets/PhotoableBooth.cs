using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhotoableBooth : MonoBehaviour
{
    public GameObject photoBoard;
    public Material photoMaterial;
    public UnityEvent OnBoothPhotographed;

    public void BoothPhotographed(Texture2D photoTexture)
    {
        photoBoard.GetComponent<Renderer>().material = photoMaterial;
        photoBoard.GetComponent<Renderer>().material.mainTexture = photoTexture;
        OnBoothPhotographed.Invoke();
    }
}
