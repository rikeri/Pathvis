using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractivePixelTracker : MonoBehaviour
{
    private InteractivePixel[] interPixels;
    private bool fulfilled = false;
    public TextMesh statsLabel;
    public UnityEvent OnAllPixelsFilled;
    // Start is called before the first frame update
    void Start()
    {
        interPixels = GetComponentsInChildren<InteractivePixel>();
        SetText(0, interPixels.Length, 0);
    }

    // called when a pixel is touched
    public void CheckPixels()
    {
        int filledPixelCount = 0;
        int totalSamples = 0;
        foreach(InteractivePixel p in interPixels) {
            totalSamples += p.samples;
            if (p.samples > 0) filledPixelCount++;
        }
        SetText(filledPixelCount, interPixels.Length, totalSamples);
        if (fulfilled) return;
        if (filledPixelCount >= interPixels.Length) {
            OnAllPixelsFilled.Invoke();
            fulfilled = true;
        }
    }

    private void SetText(int filledCount, int totalCount, int totSamples)
    {
        statsLabel.text = $"Filled pixels: {filledCount}/{totalCount}\nTotal samples: {totSamples}";
    }
}
