using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderInteractive : MonoBehaviour
{
    private bool interactionState;

    public bool InteractionFinished { set { interactionState = value; } }

    public bool InteractionSuccess { get { return interactionState; } }

    private void Update()
    {
        if (interactionState)
        {
            gameObject.layer = 0;
        }
    }
}
