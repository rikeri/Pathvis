using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GateGoalManager : MonoBehaviour
{
    private bool allFulfilled = false;
    public GoalBox[] goals; // assignable from editor
    public UnityEvent OnAllGoalsFulfilled;
    public AudioClip fulfillSound;
    private float moveDistance = 2.5f; // distance to move gate up after all goals completed
    public void FulfillGoal(int index)
    {
        if (index >= 0 && index < goals.Length)
        {
            if (goals[index] == null) 
            {
                Debug.LogWarning("No goal at specified index");
                return;
            }
            goals[index].Fulfill();
        }
        CheckAllGoals();
    }

    void CheckAllGoals()
    {
        if (allFulfilled) return;
        bool oldState = allFulfilled;
        bool newState = true;
        foreach (GoalBox goal in goals) {
            newState &= goal.goalFulfilled;
        }
        allFulfilled = newState;
        if (!oldState && allFulfilled) {
            // move gate out of the way
            StartCoroutine(MoveGate());
            AudioSource.PlayClipAtPoint(fulfillSound, transform.position, 0.5f);
            OnAllGoalsFulfilled.Invoke();
        }
    }


    IEnumerator MoveGate() {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up*moveDistance;
        float moveTime = 2f;
        for (float t = 0; t < moveTime; t += Time.deltaTime)
        {
            Vector3 pos = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, t/moveTime));
            transform.position = pos;
            yield return null;
        }

    }

    
}
