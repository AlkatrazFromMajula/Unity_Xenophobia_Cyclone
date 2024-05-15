using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallBatteryMover : MonoBehaviour
{
    Rigidbody rb;
    MagnetInteractive mi;
    bool outOfWall;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mi = GetComponent<MagnetInteractive>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mi.InteractionSuccess && !outOfWall)
        {
            Vector3 direction = transform.right;
            rb.AddForce(direction * 30f, ForceMode.Impulse);
            outOfWall = true;
        }

        // handle gravity
        rb.AddForce(Vector3.down * 2.5f, ForceMode.Force);
    }
}
