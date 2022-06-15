using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ToggleThrowable : Throwable
{   
    private bool shouldDetach = false;
    public SteamVR_Action_Boolean dropAction;
    public bool showReleaseHint = false;
    // private Player player;
    protected void Start()
    {
        // player = Player.instance;
    }

    protected override void HandAttachedUpdate(Hand hand)
    {
        // check if 
        if (dropAction[hand.handType].stateDown) shouldDetach = true;

        if (hand.IsGrabEnding(this.gameObject) && shouldDetach)
        {
            hand.DetachObject(gameObject, restoreOriginalParent);
        }

        if (onHeldUpdate != null)
            onHeldUpdate.Invoke(hand);
    }

    protected override void OnAttachedToHand(Hand hand)
    {
        // show hints on the hand we attached to
        base.OnAttachedToHand(hand);

    }

    protected override void OnDetachedFromHand(Hand hand) 
    {
        shouldDetach = false;

        base.OnDetachedFromHand(hand);
    }

    protected override void OnHandHoverBegin( Hand hand )
    {
        if (showReleaseHint) {
            ControllerButtonHints.ShowTextHint( hand, dropAction, "Pickup/Drop" );
        }
        base.OnHandHoverBegin(hand);
    }

    protected override void OnHandHoverEnd(Hand hand)
    {
        if (showReleaseHint) 
        {
            ControllerButtonHints.HideTextHint( hand, dropAction);
        }
        base.OnHandHoverEnd(hand);
    }

}
