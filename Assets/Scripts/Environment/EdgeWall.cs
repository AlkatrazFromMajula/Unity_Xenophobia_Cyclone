using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeWall : MonoBehaviour
{
    private void Start()
    {
        activateWalls(false);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Cyclone"))
        {
            if (!CyclonePhysicsManager.IsExecutingAJump)
            {
                activateWalls(true);
            }
            else
            {
                activateWalls(false);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cyclone"))
        {
            activateWalls(false);
        }
    }
    void activateWalls(bool value)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(value);
        }
    }
}
