using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrationManager : MonoBehaviour
{
    public NarratorButton currentlyPlaying = null;
    public void StartedPlaying(NarratorButton source)
    {
        if (currentlyPlaying != null) currentlyPlaying.StopNarration();
        currentlyPlaying = source;
    }
}
