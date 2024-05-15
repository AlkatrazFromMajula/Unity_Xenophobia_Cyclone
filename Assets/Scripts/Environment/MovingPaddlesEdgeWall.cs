using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPaddlesEdgeWall : EdgeWall
{
    [SerializeField] List<GameObject> wayPoints;
    [SerializeField] List<GameObject> gateWalls;

    void Update()
    {
        GameObject currentWayPoint = GetComponentInParent<MovingPaddles>().CurrentWayPoint;
        if (currentWayPoint != null)
        {
            WayPoint wayPoint = currentWayPoint.GetComponent<WayPoint>();
            if (wayPoint.PaddleHasArived)
            {
                for (int i = 0; i < wayPoints.Capacity; i++)
                {
                    if (wayPoints[i] == currentWayPoint)
                    {
                        gateWalls[i].SetActive(false);
                    }
                }
            }
        }
    }
}
