using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// from https://answers.unity.com/questions/1100566/making-a-arrow-instead-of-linerenderer.html

[RequireComponent(typeof(LineRenderer))]
public class ArrowRenderer : MonoBehaviour
{
  [Tooltip("The percent of the line that is consumed by the arrowhead")]
  [Range(0, 1)]
  public float PercentHead = 0.1f;
  public float NeckThickness = 0.2f;
    public Vector3 ArrowOrigin;
    public Vector3 ArrowTarget;
    public bool hasChanged = true;
  private LineRenderer cachedLineRenderer;
  void Start()
  {
    UpdateArrow();
  }
  private void OnValidate()
  {
    UpdateArrow();
  }
  [ContextMenu("UpdateArrow")]
  void UpdateArrow()
  {
    if (ArrowOrigin == null || ArrowTarget == null) 
        return;
    // Vector3 ArrowOrigin = Origin.position;
    // Vector3 ArrowTarget = Target.position;
    float AdaptiveSize = (float)(PercentHead / Vector3.Distance(ArrowOrigin, ArrowTarget));

    if (cachedLineRenderer == null)
      cachedLineRenderer = this.GetComponent<LineRenderer>();
    cachedLineRenderer.widthCurve = new AnimationCurve(
        new Keyframe(0, NeckThickness)
        , new Keyframe(0.999f - AdaptiveSize, NeckThickness)  // neck of arrow
        , new Keyframe(1 - AdaptiveSize, 1f)  // max width of arrow head
        , new Keyframe(1, 0f));  // tip of arrow
    cachedLineRenderer.positionCount = 4;
    cachedLineRenderer.SetPositions(new Vector3[] {
                    ArrowOrigin
                    , Vector3.Lerp(ArrowOrigin, ArrowTarget, 0.999f - AdaptiveSize)
                    , Vector3.Lerp(ArrowOrigin, ArrowTarget, 1 - AdaptiveSize)
                    , ArrowTarget });
  }

    void Update()
    {
        if (!hasChanged)
            return;
        UpdateArrow();
        hasChanged = false;
    }
}