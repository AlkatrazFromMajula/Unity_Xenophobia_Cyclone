using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    bool paddleHasArived;

    public bool PaddleHasArived { get { return paddleHasArived; } }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Moving Paddle"))
        {
            paddleHasArived = !other.gameObject.GetComponent<MovingPaddles>().IsMoving;
        }
    }
}
