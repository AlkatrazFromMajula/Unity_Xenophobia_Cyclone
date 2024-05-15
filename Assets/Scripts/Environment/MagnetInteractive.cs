using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetInteractive : MonoBehaviour
{
    private bool interactionState;

    public bool InteractionFinished { set { interactionState = value; } }

    public bool InteractionSuccess { get { return interactionState; } }
}
