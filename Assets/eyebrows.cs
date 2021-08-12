using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eyebrows : MonoBehaviour
{
    SkinnedMeshRenderer skinnedMeshRenderer;
    int bleninex;
    // Start is called before the first frame update
    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        bleninex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex("brows_up");
        Debug.Log($"Brows up has index {bleninex}");
    }

    // Update is called once per frame
    void Update()
    {
        skinnedMeshRenderer.SetBlendShapeWeight(bleninex, 1+100*Mathf.Sin(Time.time*16)/2);
    }
}
