using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingPaddles : MonoBehaviour
{
    [SerializeField] List<GameObject> path = new List<GameObject>();
    [SerializeField] float waitTime = 3.0f;
    Transform cycloneAimTransform;
    [SerializeField] bool isMoving;
    GameObject currentWayPoint;
    int pointsPassed = 0;

    public Vector3 CycloneAimPosition { get { return cycloneAimTransform.position; } }
    public bool IsMoving { get { return isMoving; } }
    public GameObject CurrentWayPoint { get { return currentWayPoint; } }

    void Start()
    {
        StartCoroutine(FollowPath());
        isMoving = true;
    }

    void Update()
    {
        
        if (pointsPassed == path.Capacity)
        {
            StartCoroutine(FollowPath());
            pointsPassed = 0;
        }
        cycloneAimTransform = transform.GetChild(0);
    }

    IEnumerator FollowPath()
    {
        foreach (GameObject wayPoint in path)
        {
            currentWayPoint = null;
            isMoving = true;
            Vector3 startPosition = transform.position;
            Vector3 endPosition = wayPoint.transform.position;
            float travelPercent = 0.0f;

            while (travelPercent < 1.0f)
            {
                travelPercent += Time.deltaTime * 0.1f;
                transform.position = Vector3.Lerp(startPosition, endPosition, travelPercent);
                yield return new WaitForEndOfFrame();
            }

            currentWayPoint = wayPoint;
            isMoving = false;
            pointsPassed++;
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }
}
