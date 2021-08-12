using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraNormalRaycast : MonoBehaviour
{
    // Attach this script to a camera and it will
    // draw a debug line pointing outward from the normal

    Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // Only if we hit something, do we continue
        RaycastHit hit;


        if (!Physics.Raycast(cam.ScreenPointToRay(new Vector3(cam.pixelWidth/2, cam.pixelHeight/2, 0)), out hit))
        {
            return;
        }

        Debug.DrawRay(hit.point, hit.normal);
        
        /*
        // Just in case, also make sure the collider also has a renderer
        // material and texture
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
        {
            return;
        }

        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;

        // Extract local space normals of the triangle we hit
        Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];

        // interpolate using the barycentric coordinate of the hitpoint
        Vector3 baryCenter = hit.barycentricCoordinate;

        // Use barycentric coordinate to interpolate normal
        Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
        // normalize the interpolated normal
        interpolatedNormal = interpolatedNormal.normalized;

        // Transform local space normals to world space
        Transform hitTransform = hit.collider.transform;
        interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);

        // Display with Debug.DrawLine
        Debug.DrawRay(hit.point, interpolatedNormal);
        */
    }
}
