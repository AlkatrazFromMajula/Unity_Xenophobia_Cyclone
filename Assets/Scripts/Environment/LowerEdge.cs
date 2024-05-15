using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerEdge : MonoBehaviour
{
    #region Fields

    protected Vector3 edgeDistance;
    protected Vector2 normalizedVector;

    #endregion

    #region Properties

    public Vector3 EdgeDistance { get { return edgeDistance; } }

    public Vector2 NormalizedVector { get { return normalizedVector; } }

    #endregion

    #region Private Methods

    protected virtual void Start()
    {
        normalizedVector = new Vector2(-transform.forward.normalized.x, -transform.forward.normalized.z);
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cyclone"))
        {
            edgeDistance = transform.GetChild(0).transform.position - other.transform.position;
        }
    }

    #endregion
}
