using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VisualTracer : MonoBehaviour
{
  public bool affectInteractive = false;
  // public bool affectButtons = false;
  public bool playSounds = false;
  public bool drawTooltips = false;
  public bool drawNormal = false; // the purple normal vector
  public bool drawScatter = false; // the yellow scatter direction vector (and sphere for lambertian)
  public bool doColorReturn = false; // if the final path should be colored in with the light that was found along it
  public bool terminateOnFirstLamp = false; // to make it more intuitively shown that path tracing tries to find paths to lights
  public bool doHitEvents = false; // should hit events on participators be invoked?
  public bool clearAfterDelay = false;
  public float clearDelay = 5f;
  public Color lineColor = new Color(0f, 0.7273f, 1.0f);
  private Vector3 rayOrigin = Vector3.zero;
  private Vector3 rayDirection = new Vector3(0, 0, 1);

  public Color background = Color.black;
  public int maxBounceDepth = 6;
  public int defaultBounceDepth = 6;
  public float maxRayLength = 5f;
  public float traceSpeed = 0.5f;
  public float shortDelayTime = 0.1f; // very short delays to separate the appearance of lines and intersection visualizers
  public float mediumDelayTime = 0.5f; // medium delay used when the entire trace is drawn and when showing random scatter
  public float returnColorSpeed = 0.3f; // time it takes for the path to get colored in 
  public float finishedDelay = 0.1f;
  public Transform skyVolumes;
  public GameObject segmentPrefab;
  public GameObject scatterVizPrefab;

  // stores the segments of the traced path
  public List<GameObject> segments = new List<GameObject>();
  public GameObject tooltipPrefab;
  public Color colorResult;
  // public GameObject traceAudioSource;
  public AudioClip fireSound;
  // public AudioClip hitSoundMirror;
  public AudioClip hitSoundLambert;
  // public AudioClip hitSoundRefractive;
  // public AudioClip hitSoundLamp;
  // public AudioClip enterSkySound;
  // public AudioClip maxBounceSound;
  public AudioClip returnSound;

  public UnityEvent onTraceDone; // when the ray is traced (instant)
  public UnityEvent onTraceVisualDone; // when the lines are drawn

  public RaycastHit firstHit; // store the first hit to use for checking goals
  public bool firstHitValid = false;


  // set the ray origin in worldspace prior to tracing (only used for the flat lamp in the start for now)
  public void SetOrigin(Vector3 worldSpacePoint)
  {
    rayOrigin = transform.InverseTransformPoint(worldSpacePoint);
  }

  // point the raytracing to a specific worldspace point prior to tracing
  public void SetDirection(Vector3 worldSpaceTarget)
  {
    rayDirection = transform.InverseTransformDirection(Vector3.Normalize(worldSpaceTarget - transform.position));
  }

  private void PlaySound(AudioClip sound, Vector3 position)
  {
    if (!playSounds) return;
    if (sound != null) AudioSource.PlayClipAtPoint(sound, position, 0.3f);
  }
  private void PlaySound(AudioClip sound, Vector3 position, float volume)
  {
    if (!playSounds) return;
    if (sound != null) AudioSource.PlayClipAtPoint(sound, position, volume);
  }

  public void ClearPath()
  {
    StopAllCoroutines();
    // clear all segments of the previous path
    foreach (GameObject s in segments)
    {
      Destroy(s);
    }
    segments.Clear();
  }

  // performs path tracing and stores the information in the segment objects
  private void TracePath()
  {
    ClearPath();


    Vector3 o = transform.TransformPoint(rayOrigin);
    Vector3 d = transform.TransformDirection(rayDirection);

    // set the background and max bounce depth depending on what skyVolume we are in
    background = Color.black;
    maxBounceDepth = defaultBounceDepth;
    foreach (Transform child in skyVolumes)
    {
      RTSkyVolume sky = child.gameObject.GetComponent<RTSkyVolume>();
      if (sky != null && sky.Contains(o))
      {
        background = sky.skyColor;
        maxBounceDepth = sky.maxBounceDepth;
        break;
      }
    }

    SegmentInfo segInfo = NewSegment(o, d);
    firstHitValid = false;
    colorResult = RayColor(o, d, maxBounceDepth, segInfo);
    onTraceDone.Invoke();
  }

  private SegmentInfo NewSegment(Vector3 origin, Vector3 direction)
  {
    GameObject firstSegment = Instantiate(segmentPrefab, origin, Quaternion.identity);
    SegmentInfo segInfo = firstSegment.GetComponent<SegmentInfo>();
    segInfo.tooltipPrefab = tooltipPrefab;
    segInfo.BeginSegment(origin, direction);
    segInfo.SetLineColor(lineColor);
    segments.Add(firstSegment);
    return segInfo;
  }

  private Color RayColor(Vector3 origin, Vector3 direction, int depth, SegmentInfo segment)
  {

    RaycastHit hit;
    if (Physics.Raycast(origin, direction, out hit, maxRayLength, 1 << 8))
    {
      // check for participator
      RaytracingParticipator rp = hit.transform.gameObject.GetComponent<RaytracingParticipator>();
      if (rp == null)
      {
        // a hit but no participator should be invalid
        segment.EndSegment(hit.point, SegmentInfo.SegmentType.Invalid);
        return background;
      }
      if (rp.materialType == RaytracingParticipator.MaterialType.RayBlocker)
      {
        // hitting a ray blocker should be identical to hitting sky
        segment.color = background;
        segment.EndSegment(hit.point, SegmentInfo.SegmentType.Sky);
        return background;
      }
      // if this is the first hit, store it
      if (depth == maxBounceDepth) {
        firstHit = hit;
        firstHitValid = true;
      }

      // calc scatter direction and store it
      Vector3 scatterDir = rp.ScatterDirection(origin, hit);

      // store the info we now know in the current segment
      segment.EndSegment(hit.point, SegmentInfo.SegmentType.Hit);
      segment.scatterDirection = scatterDir;
      segment.participator = rp;
      segment.rayHit = hit;

      // do not create a new segment if there are no more bounces
      Color nextColor = Color.black;
      if (depth - 1 <= 0)
      {
        segment.maxBounce = true; // this segment is the last in the path and should not draw hit visualizers
      }
      // or if we should terminate on the first light we find
      else if (!(terminateOnFirstLamp && rp.materialType == RaytracingParticipator.MaterialType.Lamp))
      {
        // create a new segment for the next call
        SegmentInfo nextSegment = NewSegment(hit.point, scatterDir);
        // if we hit a refractive surface, offset the start of the cast a bit
        Vector3 hitOrigin = hit.point;
        if (rp.materialType == RaytracingParticipator.MaterialType.Refractive)
        {
          hitOrigin += scatterDir * 0.001f;
        }
        nextColor = RayColor(hitOrigin, scatterDir, depth - 1, nextSegment);
      }

      // propagate color back up the recursion chain
      Color result = rp.Emitted(hit) + rp.ScatterColor(hit) * nextColor;
      segment.color = result;
      return result;

    }
    else
    {
      // ray goes into the sky, so let's draw a short segment 
      segment.color = background;
      segment.EndSegment(origin + direction, SegmentInfo.SegmentType.Sky);
      return background;
    }

  }

  private IEnumerator DrawTrace()
  {
    PlaySound(fireSound, transform.position);
    // draw every segment one by one, with speed depending on their length
    foreach (GameObject seg in segments)
    {
      SegmentInfo segInf = seg.GetComponent<SegmentInfo>();
      float traceTime = segInf.length * traceSpeed;
      float t = 0;

      

      // start looping sound
      // if (traceAudioSource != null && playSounds) {
      //   traceAudioSource.SetActive(true);
      //   // traceAudioSource.transform.position
      // }
      // draw segment
      for (; t < traceTime; t += Time.deltaTime)
      {
        Vector3 pos = segInf.LerpLine(t / traceTime);
        // if (traceAudioSource != null && playSounds) traceAudioSource.transform.position = pos;
        yield return null;
      }
      segInf.LerpLine(1); // segment fully drawn
      // if (traceAudioSource != null) {
      //   traceAudioSource.SetActive(false);
      // }
      
      yield return new WaitForSeconds(shortDelayTime);

      // invoke onhit event
      if (segInf.participator != null) 
      {
        if (doHitEvents) segInf.participator.onHit.Invoke();
        if (segInf.participator.materialType != RaytracingParticipator.MaterialType.RayBlocker)
        {
          PlaySound(hitSoundLambert, segInf.endPoint);
        }
      }
      // draw tooltip
      if (drawTooltips)
      {
        string toolText = "bounce";
        if (segInf.participator != null) 
        {
          toolText = $"{segInf.participator.materialType}";
          // AudioClip hitSound = hitSoundLambert;
          // switch (segInf.participator.materialType) {
          //   case RaytracingParticipator.MaterialType.Mirror:
          //     // hitSound = hitSoundMirror;
          //     break;
          //   case RaytracingParticipator.MaterialType.Refractive:
          //     // hitSound = hitSoundRefractive;
          //     break;
          //   case RaytracingParticipator.MaterialType.Lamp:
          //     // hitSound = hitSoundLamp;
          //     break;
          //   case RaytracingParticipator.MaterialType.RayBlocker:
          //     // hitSound = null;
          //     break;
          // }
          // PlaySound(hitSound, segInf.endPoint);
          if (segInf.participator.barrier) toolText = null; // barriers should act as invisible walls, no tooltips shown
        }
        if (segInf.segType == SegmentInfo.SegmentType.Sky) toolText = "Ray went into the sky";
        // switch (segInf.segType)
        // {
        //   case SegmentInfo.SegmentType.Maxbounce:
        //     toolText = "Reached max bounces!";
        //     // PlaySound(maxBounceSound, segInf.endPoint);
        //     break;
        //   case SegmentInfo.SegmentType.Sky:
        //     toolText = "Ray went into the sky";
        //     // PlaySound(enterSkySound, segInf.endPoint);
        //     break;
        //   default: break;
        // }
        segInf.ShowTooltip(toolText);
      }

      // draw normal 
      if (drawNormal) {
        if (segInf.participator != null && 
        segInf.segType == SegmentInfo.SegmentType.Hit && // only hits can be visualized
        (maxBounceDepth == 1 ? true : segInf.maxBounce == false) && // if we only have a single "bounce" - ie the first booth, we still want to draw normal and scatter
        segInf.participator.barrier == false) // barriers should not be visualized
        {
          // draw normal and scatter vector visualizer
          GameObject isect = Instantiate(scatterVizPrefab, segInf.endPoint, Quaternion.identity);
          IntersectVisualizer iv = isect.GetComponent<IntersectVisualizer>();
          iv.Initialize(segInf.rayHit, segInf.participator, segInf.direction, !drawScatter);
          isect.transform.SetParent(seg.transform);

          // show some random scatter directions before snapping to the precalculated one
          if (drawScatter)
          {
            iv.UpdateScatterArrow(segInf.scatterDirection);
            t = 0;
            for (; t < mediumDelayTime; t += Time.deltaTime)
            {
            //   iv.UpdateScatterArrow(segInf.participator.ScatterDirection(segInf.startPoint, segInf.rayHit));
              yield return null;
            }
          }
        }
      }

      yield return new WaitForSeconds(shortDelayTime);
    }
    yield return new WaitForSeconds(mediumDelayTime);

    if (doColorReturn) {
      for (int i = segments.Count - 1; i >= 0; i--)
      {
        GameObject seg = segments[i];
        SegmentInfo segInf = seg.GetComponent<SegmentInfo>();
        segInf.UpdateLineColor();
        PlaySound(returnSound, Vector3.Lerp(segInf.startPoint, segInf.endPoint, 0.5f), 0.15f);
        yield return new WaitForSeconds(returnColorSpeed);
      }
    }

    // used for the demo booth at the very start, color the path white if it connects with the light
    if (!doColorReturn && terminateOnFirstLamp && colorResult.maxColorComponent > 0.1f) 
    {
      for (int i = segments.Count - 1; i >= 0; i--)
      {
        GameObject seg = segments[i];
        SegmentInfo segInf = seg.GetComponent<SegmentInfo>();
        segInf.SetLineColor(Color.white);
      }
    }

    yield return new WaitForSeconds(finishedDelay);
    // visual tracing has completely finished, invoko to signal that the color is usable
    onTraceVisualDone.Invoke();

    // check if first ray passed through the pixel grid camera
    if (affectInteractive && segments.Count >= 1) {
      UpdateInteractivePixel(segments[0].GetComponent<SegmentInfo>());
    }

    if (clearAfterDelay) StartCoroutine(DelayClear());
  }

  private void UpdateInteractivePixel(SegmentInfo firstPathSegment)
  {
    // raytrace in the interactive layer to see if this ray went through the origin and a pixel
    RaycastHit[] hits;
    hits = Physics.RaycastAll(firstPathSegment.startPoint, firstPathSegment.direction, firstPathSegment.length, 1 << 10);
    if (hits.Length < 2) return; // not enough hits
    bool foundOrigin = false;
    RaycastHit closestHit = hits[0];
    float closestHitDistance = 100f;
    for (int k=0; k<hits.Length; k++)
    {
      RaycastHit hit = hits[k]; 
      InteractivePixel potentialPixel = hit.transform.GetComponent<InteractivePixel>();
      if (potentialPixel == null) {
        foundOrigin = true;
      }
      else if (hit.distance < closestHitDistance)
      {
        closestHit = hit;
        closestHitDistance = hit.distance;
      }
    }
    if (!foundOrigin) return;
    closestHit.transform.GetComponent<InteractivePixel>().AddSample(colorResult);
  }

  private IEnumerator DelayClear()
  {
    yield return new WaitForSeconds(clearDelay);
    ClearPath();
  }

  public void DoTrace()
  { // callable from elsewhere
    StopAllCoroutines();
    TracePath();
    StartCoroutine(DrawTrace());
  }
}
