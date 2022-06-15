using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractivePixel : MonoBehaviour
{
    public int samples;
    private Color accuColor; // accumulative color
    private Color displayColor; // color used in rendered material
    private Material material;
    public UnityEvent OnPixelTouched; // used to check for goal
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
        ResetPixel();
    }

    public void ResetPixel()
    {
        samples = 0;
        accuColor = Color.black;
        displayColor = new Color(0f,0f,0f,0f);
        UpdateMaterial();
    }

    public void AddSample(Color color)
    {
        samples++;
        accuColor += color;
        displayColor = accuColor / samples;
        UpdateMaterial();
        OnPixelTouched.Invoke();
    }

    private void UpdateMaterial()
    {
        material.color = displayColor;
    }
}
