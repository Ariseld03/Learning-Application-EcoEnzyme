using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoAnimationClosing : MonoBehaviour
{
    public ClosingAnimationEvents closingEvents;

    void Start()
    {
        closingEvents.StartClosing();
    }
}
