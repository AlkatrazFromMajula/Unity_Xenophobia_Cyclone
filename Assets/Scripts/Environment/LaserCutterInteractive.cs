using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCutterInteractive : MonoBehaviour
{
    [SerializeField] GameObject damagedWire;
    private bool touchedByLaser;

    public bool TouchedByLaser { set { touchedByLaser = value; } }

    private void FixedUpdate()
    {
        if (touchedByLaser)
        {
            if (gameObject.CompareTag("CuttableWire"))
            {
                damagedWire.SetActive(true);
                Destroy(gameObject);
            }
            else if (gameObject.CompareTag("ThisThing"))
            {
                Destroy(gameObject);
            }
        }
    }
}
