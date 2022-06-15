using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jitterer : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public Transform affectedObj;
    public float speed = 1f;
    private Vector3 fromPoint;
    private Vector3 toPoint;
    private bool moving = false;

    IEnumerator MoveToPoint()
    {
        float moveTime = speed;
        for (float t = 0; t < moveTime; t+= Time.deltaTime)
        {
            affectedObj.position = Vector3.Lerp(fromPoint, toPoint, t/moveTime);
            yield return null;
        }
        affectedObj.position = toPoint;
        moving = false;
    }

    Vector3 RandomPoint() {
        return new Vector3(Random.Range(pointA.position.x, pointB.position.x), 
                           Random.Range(pointA.position.y, pointB.position.y), 
                           Random.Range(pointA.position.z, pointB.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        if (!moving) {
            fromPoint = affectedObj.position;
            toPoint = RandomPoint();
            moving = true;
            StartCoroutine(MoveToPoint());
        }
    }
}
