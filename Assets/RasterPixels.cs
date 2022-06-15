using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pixel
{
  public Pixel(Vector3 p)
  {
    // G = g;
    pos = p;
    accum = 0.0f;
    colores = Color.black;
  }
  // public GameObject G { get; set; }
  public Vector3 pos { get; set; }
  public float accum { get; set; }
  public Color colores {get; set;}

}

public enum RenderType {
    PathTracing,
    UVs,
    Normals,
    SolidColors,
    SimpleRT
};

public class RasterPixels : MonoBehaviour
{
  public float pixelScale = 0.1f;
  public int hPixels = 32;
  public int vPixels = 24;
  public Transform cameraOrigin;
  private Vector3 cameraOriginRestPosition; 
  private Quaternion cameraOriginRestRotation; 
  private Pixel[,] pixels;
  private uint samples = 0;
  private Texture2D resultTex;
  public Material targetMat;
  public GameObject viewport;
  public Color background;
  public int maxDepth = 6;
  public float maxRayLength = 5f;
  public RenderType renderMode = RenderType.PathTracing;
  public Transform skyVolumes;

  private int _vPixels;
  private int _hPixels;
  private float _pixelScale;
  public bool cameraEnabled = false;
  public int isHeld = 0;

  public TextMesh statsLabel;
  private float startTime;

  private float frameStartTime;
  public float maxFrameDuration = 1f / 120f;
  public UnityEvent onModeChange;

  // Start is called before the first frame update
  void Start()
  {
    _pixelScale = pixelScale;
    _hPixels = hPixels;
    _vPixels = vPixels;
    cameraOriginRestPosition = cameraOrigin.localPosition;
    cameraOriginRestRotation = cameraOrigin.localRotation;
    Restart();
  }

  public void ResetOrigin()
  {
    cameraOrigin.localPosition = cameraOriginRestPosition;
    cameraOrigin.localRotation = cameraOriginRestRotation;
    cameraOrigin.hasChanged = true;
  }

  public Texture2D PrintTexture()
  {
    resultTex.Apply(true); // write mipchain
    Texture2D copyTex = new Texture2D(resultTex.width, resultTex.height, TextureFormat.RGBAFloat, true, true);
    Graphics.CopyTexture(resultTex, copyTex);
    return copyTex;
  }

  void Restart() {

    samples = 0;
    resultTex = new Texture2D(hPixels+1, vPixels+1, TextureFormat.RGBAFloat, true, true);
    resultTex.filterMode = hPixels > 64 ? FilterMode.Trilinear : FilterMode.Point;
    resultTex.wrapMode = TextureWrapMode.Clamp;
    targetMat.mainTexture = resultTex;
    pixels = new Pixel[hPixels + 1, vPixels + 1];

    viewport.transform.localScale = new Vector3((hPixels+1) * pixelScale, (vPixels+1) * pixelScale, 1.0f);

    Vector3 centering = new Vector3(0.0f, 0f + vPixels * pixelScale,
                                        0f + hPixels * pixelScale) / 2f;
    for (int w = 0; w <= hPixels; w++)
      for (int h = 0; h <= vPixels; h++)
      {
        Vector3 ppos = new Vector3(0f,
                                   0f + h * pixelScale,
                                   0f + w * pixelScale) - centering;
        pixels[w, h] = new Pixel(ppos);
      }

    FrustrumUpdate();
    CheckForSky();
    startTime = Time.time;
  }

  string FormatSeconds(float elapsed)
  {
      int d = (int)(elapsed * 100.0f);
      int minutes = d / (60 * 100);
      int seconds = (d % (60 * 100)) / 100;
      int hundredths = d % 100;
      return System.String.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, hundredths);
  }

  public void UpdateStats() {
    statsLabel.text = StatsString();
  }

  public string StatsString() {
    return $"samples: {samples}\ntime: {FormatSeconds(Time.time - startTime)}";
  }

  public void ChangeResolution(int horizpix, int verticpix, float pixsize, bool overwrite=false) {
    StopAllCoroutines();
    if (overwrite) {
      _hPixels = horizpix;
      _vPixels = verticpix;
      _pixelScale = pixsize;
    }
    isRunning = false;
    hPixels = horizpix;
    vPixels = verticpix;
    pixelScale = pixsize;
    Restart();
  }

void FrustrumUpdate()
{
  Vector3 centering = new Vector3(0.0f, 0f + vPixels * pixelScale,
                                        0f + hPixels * pixelScale) / 2f;
  float ps = pixelScale / 2;
  Vector3 bottomright = transform.TransformPoint(new Vector3(0.0f, -ps, hPixels * pixelScale + ps) - centering);
  Vector3 topright = transform.TransformPoint(new Vector3(0.0f, vPixels * pixelScale + ps, hPixels * pixelScale + ps) - centering);
  Vector3 topleft = transform.TransformPoint(new Vector3(0.0f, vPixels * pixelScale + ps, -ps) - centering);
  Vector3 bottomLeft = transform.TransformPoint(new Vector3(0.0f, -ps, -ps) - centering);
  Vector3 camorig = cameraOrigin.position;

  LineRenderer lr = this.GetComponent<LineRenderer>();
  lr.positionCount = 10;
  lr.SetPositions(new Vector3[] {
              bottomLeft,
              topleft,
              camorig,
              topright,
              bottomright,
              camorig,
              bottomLeft,
              bottomright,
              topright,
              topleft});
}

public void OnGrabbed() {
  StopAllCoroutines();
  if (isHeld == 0) {
    _pixelScale = pixelScale;
    _hPixels = hPixels;
    _vPixels = vPixels;
  }
  isHeld += 1;
  ChangeResolution(32, 24, 0.04f);
}

public void OnReleased() {
  isHeld -= 1;
  if (isHeld < 1) ChangeResolution(_hPixels, _vPixels, _pixelScale);
}

public void SetRenderMode(RenderType mode) {
  if (renderMode == mode) return;
  renderMode = mode;
  if(isHeld < 1) ChangeResolution(_hPixels, _vPixels, _pixelScale);
  onModeChange.Invoke();
}

Color RayColor(Vector3 origin, Vector3 direction, int depth) {
  if (depth <= 0) {
    return new Color(0f,0f,0f,1f);
  }
  RaycastHit hit;
  if (Physics.Raycast(origin, direction, out hit, maxRayLength, 1<<8))
  {
    // check for participator
    RaytracingParticipator rp = hit.transform.gameObject.GetComponent<RaytracingParticipator>();
    if (rp == null) {
      return background;
    }
    if (rp.materialType == RaytracingParticipator.MaterialType.RayBlocker) {
      return background;
    }
    // if we hit a refractive surface, offset the start of the cast a bit
    Vector3 hitOrigin = hit.point;
    Vector3 scatterDir = rp.ScatterDirection(origin, hit);
    if (rp.materialType == RaytracingParticipator.MaterialType.Refractive)
    {
      hitOrigin += scatterDir * 0.001f;
    }

    // the recursive rendering function
    return rp.Emitted(hit) + rp.ScatterColor(hit) * RayColor(hitOrigin, scatterDir, depth-1);

  } else{
    // Debug.DrawRay(origin, direction, Color.red, 0.5f);
    return background;
  }
}

Color SimpleRayColor(Vector3 origin, Vector3 direction, int depth) {
  if (depth <= 0) {
    return new Color(0f,0f,0f,1f);
  }
  RaycastHit hit;
  if (Physics.Raycast(origin, direction, out hit, maxRayLength, 1<<8))
  {
    // check for participator
    RaytracingParticipator rp = hit.transform.gameObject.GetComponent<RaytracingParticipator>();
    if (rp == null) {
      return background;
    }
    if (rp.materialType == RaytracingParticipator.MaterialType.RayBlocker) {
      return background;
    }
    // apply a simple shading
    float factor = Vector3.Dot(hit.normal, new Vector3(0.3f, 1f, 0.8f))/ 3f + 0.5f;

    // if this material is reflective or refractive, continue bouncing
    if (rp.materialType == RaytracingParticipator.MaterialType.Mirror || 
        rp.materialType == RaytracingParticipator.MaterialType.Refractive) {
        // if we hit a refractive surface, offset the start of the cast a bit
        Vector3 hitOrigin = hit.point;
        Vector3 scatterDir = rp.ScatterDirection(origin, hit);
        if (rp.materialType == RaytracingParticipator.MaterialType.Refractive)
        {
          hitOrigin += scatterDir * 0.001f;
        }
        return rp.Emitted(hit) + rp.ScatterColor(hit) * factor * SimpleRayColor(hitOrigin, scatterDir, depth-1);
    }

    return rp.Emitted(hit) + rp.ScatterColor(hit) * factor;

  } else{
    return background;
  }
}

Color NormalRayColor(Vector3 origin, Vector3 direction) {
  RaycastHit hit;
  if (Physics.Raycast(origin, direction, out hit, maxRayLength, 1<<8))
  {
    // check for participator
    RaytracingParticipator rp = hit.transform.gameObject.GetComponent<RaytracingParticipator>();
    if (rp == null) {
      return background;
    }
    if (rp.materialType == RaytracingParticipator.MaterialType.RayBlocker) {
      return background;
    }
    Vector3 n = (hit.normal + new Vector3(1f, 1f, 1f))/2f;
    return new Color(n.x, n.y, n.z, 1f);

  } else{
    return background;
  }
}

Color UVRayColor(Vector3 origin, Vector3 direction) {
  RaycastHit hit;
  if (Physics.Raycast(origin, direction, out hit, maxRayLength, 1<<8))
  {
    // check for participator
    RaytracingParticipator rp = hit.transform.gameObject.GetComponent<RaytracingParticipator>();
    if (rp == null) {
      return background;
    }
    if (rp.materialType == RaytracingParticipator.MaterialType.RayBlocker) {
      return background;
    }
    return new Color(hit.textureCoord.x, hit.textureCoord.y, 0f, 1f);

  } else{
    return background;
  }
}

bool isRunning = false;
IEnumerator RayTrace()
{
    if (!cameraEnabled) yield break;
    isRunning = true;

    if (transform.hasChanged)
    {
        foreach (Pixel px in pixels) px.colores = Color.black;
        samples = 0;
        startTime = Time.time;
    }
    transform.hasChanged = false;
    samples += 1;

    frameStartTime = Time.realtimeSinceStartup;
    for (int h = 0; h <= vPixels; h++){
      for (int w = 0; w <= hPixels; w++)
        {
          Pixel px = pixels[w, h];
          Vector3 pos = transform.TransformPoint(px.pos);
          Vector2 texcoord = new Vector2(0f,0f);
          Color result;
          switch (renderMode) {
            case RenderType.PathTracing:
              result = RayColor(cameraOrigin.position, (pos - cameraOrigin.position).normalized, maxDepth);
              break;
            case RenderType.SimpleRT:
            default:
              result = SimpleRayColor(cameraOrigin.position, (pos - cameraOrigin.position).normalized, maxDepth);
              break;
            case RenderType.Normals:
              result = NormalRayColor(cameraOrigin.position, (pos - cameraOrigin.position).normalized);
              break;
            case RenderType.UVs:
              result = UVRayColor(cameraOrigin.position, (pos - cameraOrigin.position).normalized);
              break;
          }
          Color finalPix;
          px.colores += result;
          finalPix = px.colores / samples;
          resultTex.SetPixel(w, h, finalPix); // SetPixels is probably faster, but seems to use an odd format
      }
      resultTex.Apply(false);
      UpdateStats();
      // check how we are doing on the frametime 
      float currentTime = Time.realtimeSinceStartup;
      if (currentTime - frameStartTime > maxFrameDuration) {
        // Debug.Log($"current time above maxframeduration, yielding");
        yield return null; // only trace a horizontal slice of pixels each frame
        frameStartTime = Time.realtimeSinceStartup;
      } else {
        // Debug.Log($"Continued rendering next slice");
      }
    }
    // yield return new WaitForSeconds(0.5f);
    isRunning = false;
    resultTex.Apply(true);
}

void TraceRealtime()
{
    if (!transform.hasChanged)
      return;

    foreach (Pixel px in pixels) px.colores = Color.black;

    for (int h = 0; h <= vPixels; h++){
      for (int w = 0; w <= hPixels; w++)
        {
          Pixel px = pixels[w, h];
          Vector3 pos = transform.TransformPoint(px.pos);

          Color finalPix = SimpleRayColor(cameraOrigin.position, (pos - cameraOrigin.position).normalized, 2);
          resultTex.SetPixel(w, h, finalPix); // SetPixels is probably faster, but seems to use an odd format
      }
    }
    resultTex.Apply(true);
}

  // Update is called once per frame
  void Update()
  {
    // control when to restart rendering (whenever the camera moves)
    if (cameraOrigin.hasChanged)
    {
        transform.hasChanged = true;
        cameraOrigin.hasChanged = false;
    }
    if (transform.hasChanged)
    {
        FrustrumUpdate();
        CheckForSky();
    }
    if (isHeld > 0) {
      TraceRealtime();
    } else {
      if (cameraEnabled && !isRunning) StartCoroutine(RayTrace());
    }
    // RayTrace();
  }
  private void CheckForSky() {
    background = Color.black;
    foreach (Transform child in skyVolumes)
    {
        RTSkyVolume sky = child.gameObject.GetComponent<RTSkyVolume>();
        if (sky != null && sky.Contains(cameraOrigin.position)) {
          background = sky.skyColor;
          break;
        }
    }
  }

  public void ToggleCameraEnabled() {
    cameraEnabled = !cameraEnabled;
    if (!cameraEnabled) { 
      StopAllCoroutines();
      isRunning = false;
      }
  }
}
