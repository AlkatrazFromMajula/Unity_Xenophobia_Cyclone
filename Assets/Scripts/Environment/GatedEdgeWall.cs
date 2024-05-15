using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatedEdgeWall : EdgeWall
{
    [SerializeField] GameObject wayPoint;
    [SerializeField] GameObject gateWall;

    void Update()
    {
        if (wayPoint.GetComponent<WayPoint>().PaddleHasArived)
        {
            gateWall.SetActive(false);
        }
    }
}
