using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryBigMover : MonoBehaviour
{
    MagnetInteractive mi;
    Vector3 finalPosition;

    private void Start()
    {
        mi = GetComponent<MagnetInteractive>();
        finalPosition = transform.position + transform.right * 8;
    }

    IEnumerator Move()
    {
        float pathProgress = 0;

        while (pathProgress <= 1)
        {
            pathProgress += Time.deltaTime * 2;
            transform.position = Vector3.Lerp(transform.position, finalPosition, pathProgress);

            yield return new WaitForEndOfFrame();
        }
        Destroy(mi);
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (mi.InteractionSuccess)
        {
            StartCoroutine(Move());
            mi.InteractionFinished = false;
        }
    }
}
