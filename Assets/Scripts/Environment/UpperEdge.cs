using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperEdge : LowerEdge
{
    protected override void OnTriggerStay(Collider other)
    {
        edgeDistance = transform.localPosition;
    }

    protected override void Start()
    {
        normalizedVector = new Vector2(transform.forward.normalized.x, transform.forward.normalized.z);
    }
}
