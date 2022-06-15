using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowRasterCamera : MonoBehaviour
{
  public float pixelScale = 0.03f;
  public int hPixels = 15;
  public int vPixels = 10;
  private int x = 0;
  private int y = 0;
  private Pixel[,] pixels;
  public GameObject viewport;
  private uint samples = 1;
  private Texture2D resultTex;
  public Material targetMat;
  public VisualTracer visTracer;
  // private bool tracing = true;
  private bool beginNextTrace = true;
  public bool tracingActive = true;

  public LineRenderer frustum;
  // Start is called before the first frame update
  void Start()
  {
    y = vPixels;
    targetMat = viewport.GetComponent<Renderer>().material;
    frustum = GetComponent<LineRenderer>();
    InitRaster();
    // beginNextTrace = true;
    visTracer.onTraceVisualDone.AddListener(TraceFinished);
  }
  public void ToggleTracing()
  {
    tracingActive = !tracingActive;
  }

  public void SetTracing(bool state)
  {
    tracingActive = state;
  }

  void InitRaster()
  {
    samples = 1;
    resultTex = new Texture2D(hPixels+1, vPixels+1, TextureFormat.RGBAFloat, true, true);
    resultTex.filterMode = FilterMode.Point;
    resultTex.wrapMode = TextureWrapMode.Clamp;
    targetMat.mainTexture = resultTex;
    pixels = new Pixel[hPixels + 1, vPixels + 1];

    viewport.transform.localScale = new Vector3((hPixels+1) * pixelScale, (vPixels+1) * pixelScale, 1.0f);

    Vector3 centering = new Vector3(0f + hPixels * pixelScale, 0f + vPixels * pixelScale,
                                        0) / 2f;
    for (int w = 0; w <= hPixels; w++)
      for (int h = 0; h <= vPixels; h++)
      {
        Vector3 ppos = new Vector3(0f + w * pixelScale,
                                   0f + h * pixelScale,
                                   0f) - centering;
        pixels[w, h] = new Pixel(ppos);
      }
    UpdateFrustum();
  }

  void UpdateFrustum()
  {
    Vector3 centering = new Vector3(0f + hPixels * pixelScale, 0f + vPixels * pixelScale,
                                        0) / 2f;
    float ps = pixelScale / 2;
    Vector3 bottomright = transform.TransformPoint(new Vector3(hPixels * pixelScale + ps, -ps, 0f) - centering);
    Vector3 topright = transform.TransformPoint(new Vector3(hPixels * pixelScale + ps, vPixels * pixelScale + ps, 0f) - centering);
    Vector3 topleft = transform.TransformPoint(new Vector3(-ps, vPixels * pixelScale + ps, 0f) - centering);
    Vector3 bottomLeft = transform.TransformPoint(new Vector3(-ps, -ps, 0f) - centering);
    frustum.positionCount = 4;
    frustum.SetPositions(new Vector3[] {
              bottomLeft,
              topleft,
              topright,
              bottomright});
  }

  void TraceFinished()
  {
    Pixel px = pixels[x, y];
    px.colores += visTracer.colorResult;
    // Debug.Log($"Pix [{x},{y}]: {px.colores}");
    Color finalPix = px.colores/samples;
    finalPix.a = 1.0f;
    resultTex.SetPixel(x, y, finalPix);
    resultTex.Apply(true);

    x++;
    beginNextTrace = true;
    if (x>hPixels) {
      x = 0;
      y--;
    }
    if (y < 0) {
      // tracing = false;
      // Debug.Log("Finished all pixels");
      samples ++;
      y = vPixels;
      x = 0;
    }
    beginNextTrace = true;
  }

  void BeginPixelTrace()
  {
    Pixel px = pixels[x, y];
    Vector3 pxWorldSpacePos = transform.TransformPoint(px.pos);
    // Debug.Log($"Shooting at pixel {x},{y} at local pos {px.pos} in world space {pxWorldSpacePos}");
    visTracer.SetDirection(pxWorldSpacePos);
    visTracer.DoTrace();
  }

  // Update is called once per frame
  void Update()
  {
    if (!tracingActive) return;
    if (beginNextTrace) {
      beginNextTrace = false;
      BeginPixelTrace();
    }
    // for (int h = 0; h <= vPixels; h++){
    //   for (int w = 0; w <= hPixels; w++)
    //     {
    //       Pixel px = pixels[w, h];
    //       Vector3 pos = transform.TransformPoint(px.pos);
    //       Debug.DrawLine(visTracer.transform.position, pos);
          
    //   }
    // }
  }
}
