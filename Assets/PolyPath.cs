using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint
{
  public PathPoint(Vector3 p, Color c)
  {
    pos = p;
    color = c;
  }
    public PathPoint(Vector3 p)
  {
    pos = p;
    color = Color.black;
  }
  public Vector3 pos { get; set; }
  public Color color {get; set;}
}

public class PolyPath : MonoBehaviour
{
    public List<PathPoint> points;
    // private int numPoints;

    public LineRenderer lineR;
    // Start is called before the first frame update
    public GameObject colorViz;
    private GameObject ball;
    public float traceTime = 1f;
    public float returnTime = 3f;
    void Start()
    {
        // // LineRenderer lr = this.gameObject.GetComponent<LineRenderer>();
        // points = new List<PathPoint>();
        // int numPoints = lineR.positionCount;
        // // grab the points from the linerenderer
        // Vector3[] positions = new Vector3[numPoints];
        // lineR.GetPositions(positions);
        // foreach (Vector3 p in positions) {
        //     points.Add(new PathPoint(p, Random.ColorHSV(0f, 1f, 0.5f, 1.0f, 0.5f, 1.0f)));
        // }
        // StartCoroutine(DrawPath());
    }

    // returns a position interpolated along the entire path
    Vector3 polyLerp(float t) {
        t = Mathf.Clamp(t, 0.0001f, 0.9999f);
        int numPoints = points.Count;
        if (numPoints <= 0) {
            return new Vector3(0,0,0);
        }
        else if (numPoints == 1) { // only one point
            return points[0].pos;
        }
        else if (numPoints == 2) { // simple lerp
            return Vector3.Lerp(points[0].pos, points[1].pos, t);
        }
        else {
            int segments = numPoints - 1;
            t *= segments;
            int offset = Mathf.FloorToInt(t);
            float sub_t = t-offset; // lerp parameter along the selected segment
            return Vector3.Lerp(points[offset].pos, points[offset+1].pos, sub_t);
        }
    }

    // returns a list of positions and position count that can be fed into a linerenderer
    (Vector3[], int) linePoints(float t) {
        t = Mathf.Clamp(t, 0.0001f, 0.9999f);
        int numPoints = points.Count;
        int total_segments = numPoints - 1;
        int full_segments = Mathf.FloorToInt(t * total_segments);
        int posCount = full_segments + 2;
        Vector3[] newPoints = new Vector3[posCount];
        for (int i = 0; i <= posCount-1; i++) {
            newPoints[i] = points[i].pos;
        }
        newPoints[posCount-1] = polyLerp(t);
        return (newPoints, posCount);
    }


    // Update is called once per frame
    void Update()
    {
        // int numPoints = points.Count;
        // float lerpfac = (Mathf.Sin(Time.time*0.3f)+1f)/2f;

        // (Vector3[] np, int pc) = linePoints(lerpfac);
        // lr.positionCount = pc;
        // lr.SetPositions(np);

        // transform.position = polyLerp(lerpfac); // update transform of this object (ball)
        // int colorIndx = Mathf.CeilToInt(lerpfac * (numPoints-1));

        // gameObject.GetComponent<Renderer>().material.color = points[colorIndx].color;
    }

    public void StartDrawing()
    {
        StartCoroutine(DrawPath());
    }
    public void StopDrawing()
    {
        StopAllCoroutines();
        Destroy(ball);
    }

    IEnumerator DrawPath()
    {
        float t = 0;
        for (; t < traceTime; t += Time.deltaTime) {
            (Vector3[] np, int pc) = linePoints(t/traceTime);
            lineR.positionCount = pc;
            lineR.SetPositions(np);
            yield return null;
        }
        // give the full path to the linerenderer
        Vector3[] finalPositions = new Vector3[points.Count];
        for (int i = 0; i <= points.Count-1; i++) {
            finalPositions[i] = points[i].pos;
        }
        lineR.positionCount = points.Count;
        lineR.SetPositions(finalPositions);
        StartCoroutine(ReturnBall());
    }

    // instantiate a ball at the end of the path, animate it returning along the path
    // and picking up colors
        IEnumerator ReturnBall()
    {
        float t = 0;
        // float time = returnTime;
        ball = Instantiate(colorViz, points[points.Count-1].pos, Quaternion.identity);
        for (; t < returnTime; t += Time.deltaTime) {
            float lerpfac = 1-t/returnTime;
            ball.transform.position = polyLerp(lerpfac);
            int colorIndx = Mathf.CeilToInt(lerpfac * (points.Count-1));
            ball.GetComponent<Renderer>().material.color = points[colorIndx].color;    
            yield return null;
        }
        Destroy(ball);
    }
}
