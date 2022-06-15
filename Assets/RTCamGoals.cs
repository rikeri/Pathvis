using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RTCamGoals : MonoBehaviour
{
    public RasterPixels rtCamera;
    public RasterPrinter printer;
    public UnityEvent onPrintTwoPhotos;
    public UnityEvent onTrySimpleRT;
    private int photosPrinted = 0;
    // Start is called before the first frame update
    void Start()
    {
        rtCamera.onModeChange.AddListener(ModeTracker);
        printer.onSuccessfulPrint.AddListener(PhotoCounter);
    }

    private void ModeTracker()
    {
        if (rtCamera.renderMode == RenderType.SimpleRT)
        {
            onTrySimpleRT.Invoke();
            rtCamera.onModeChange.RemoveListener(ModeTracker);
        }
    }

    private void PhotoCounter()
    {
        photosPrinted += 1;
        if (photosPrinted >= 2) {
            onPrintTwoPhotos.Invoke();
            printer.onSuccessfulPrint.RemoveListener(PhotoCounter);
        }
    }
}
