using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderImmobilizer : MonoBehaviour
{
    private bool immobilize;
    private bool prohibitQuit;

    public bool Immobilize { set { immobilize = value; } }
    public bool ProhibitQuit { set { prohibitQuit = value; } }

    public bool QuitProhibited { get { return prohibitQuit; } }
    public bool Immobilized { get { return immobilize; } }

    private void Start()
    {
        immobilize = false;
        prohibitQuit = false;
    }
}
