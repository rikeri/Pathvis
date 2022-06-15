using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class SegmentInfo : MonoBehaviour
{
    public enum SegmentType {
        Hit,
        Sky,
        Invalid
    };
    public Vector3 startPoint;
    public Vector3 direction;
    public Vector3 endPoint;
    public bool maxBounce; // if this is the last segment in a path
    public SegmentType segType;
    public RaytracingParticipator participator;
    public RaycastHit rayHit;
    public Vector3 scatterDirection;
    public float length;
    public Color color; // color calculated at hitpoint
    private LineRenderer line;
    private float lineWidth = 0.008f;
    public GameObject tooltipPrefab;

    // initialize the segmentInfo after instantiation
    public void BeginSegment(Vector3 start, Vector3 dir) {
        line = this.gameObject.GetComponent<LineRenderer>();
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        startPoint = start;
        direction = dir;

    }

    // fill in the rest of the info when we know it
    public void EndSegment(Vector3 end, SegmentType type) {
        endPoint = end;
        length = Vector3.Distance(startPoint, end);
        segType = type;
        if (segType == SegmentType.Invalid) {
            return;
        } else {
            Vector3[] points = {startPoint, endPoint};
            line.SetPositions(points);
        }
        if (segType == SegmentType.Sky) {
            line.endColor = color * new Color(1,1,1,0); //fade to transparent
        }
    }

    // force the line color to a specific one
    public void SetLineColor(Color color) {
        line.startColor = color;
        line.endColor = color;
    }

    public void UpdateLineColor() {
        line.endColor = color;
        if (segType == SegmentType.Sky) {
            line.endColor = color * new Color(1,1,1,0); //fade to transparent
        }
        line.startColor = color;
        // switch (segType) {
        //     case SegmentType.Hit:
        //         line.material.color = Color.white;
        //         break;
        //     case SegmentType.Sky:
        //         line.startColor = Color.cyan;
        //         break;
        //     case SegmentType.Maxbounce:
        //         line.material.color = Color.red;
        //         break;
        //     default:
        //         line.material.color = Color.black;
        //         break;
        // }
        // if (segType != SegmentType.Invalid)
        //     line.enabled = true;
        return;
    }

    public Vector3 LerpLine(float t) {
        if (line == null) return startPoint;
        Vector3 lerpPoint = Vector3.Lerp(startPoint, endPoint, t);
        Vector3[] points = {startPoint, lerpPoint};
        line.SetPositions(points);
        line.enabled = true;
        return lerpPoint;
    }

    public void ShowTooltip(string text) {
        if (text == null) return;
        Vector3 tipPlacement = endPoint;
        if (segType == SegmentType.Sky) tipPlacement = Vector3.Lerp(startPoint, endPoint, 0.5f);

        GameObject tooltip = Instantiate(tooltipPrefab, tipPlacement, Quaternion.identity);
        tooltip.transform.SetParent(this.transform);
        foreach (TextMesh tm in tooltip.GetComponentsInChildren<TextMesh>()) {
            tm.text = text;
        }
        tooltip.SetActive(true);
    }

    // // Start is called before the first frame update
    // void Start()
    // {
    //     line = this.gameObject.GetComponent<LineRenderer>();
    // }
}
