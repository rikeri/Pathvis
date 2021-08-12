using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawmealine : MonoBehaviour
{
//   public GameObject gameObject1;          // Reference to the first GameObject
//   public GameObject gameObject2;          // Reference to the second GameObject
  public GameObject kidParent;

  private LineRenderer line;                           // Line Renderer

  // Use this for initialization
  void Start()
  {
    // determine which objects to draw the line between
    kidParent = GameObject.Find("LineKids");
    // Add a Line Renderer to the GameObject
    line = this.gameObject.AddComponent<LineRenderer>();
    // Set the width of the Line Renderer
    line.startWidth = 0.05F;
    line.endWidth = 0.05F;
    // Set the number of vertex fo the Line Renderer
    line.positionCount = kidParent.transform.childCount;
  }

  // Update is called once per frame
  void Update()
  {
    // Check if the GameObjects are not null
    if (kidParent != null)
    {
      // Update position of the two vertex of the Line Renderer
      line.positionCount = kidParent.transform.childCount;
      for (int i = 0; i < kidParent.transform.childCount; i++) {
          line.SetPosition(i, kidParent.transform.GetChild(i).position);
      }
    //   line.SetPosition(0, gameObject1.transform.position);
    //   line.SetPosition(1, gameObject2.transform.position);
    }
  }
}
