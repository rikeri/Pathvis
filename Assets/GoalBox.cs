using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent( typeof( Image ) )]
public class GoalBox : MonoBehaviour
{
    public Sprite texChecked;
    public Sprite texUnchecked;

    private Image imageComponent;
    public bool goalFulfilled = false;
    public AudioClip goalSound;
    
    public void Awake()
    {
        imageComponent = GetComponent<Image>();
    }

    public void Fulfill()
    {
        UpdateGoalState(true);
    }

    private void UpdateGoalState(bool state)
    {
        bool prev_state = goalFulfilled;
        goalFulfilled = state;
        imageComponent.sprite = state ? texChecked : texUnchecked;
        if (!prev_state && state) AudioSource.PlayClipAtPoint(goalSound, transform.position, 0.5f);
    }
}
