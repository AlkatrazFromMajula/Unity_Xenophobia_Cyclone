using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterAim : MonoBehaviour
{
    [SerializeField] Transform cam;
    [SerializeField] Transform target;
    LayerMask mask;

    private void Awake()
    {
        mask = LayerMask.GetMask("Cyclone");
    }

    // Update is called once per frame
    void Update()
    {
        Ray centralAimRay = new Ray(cam.position, cam.forward);
        if (Physics.Raycast(centralAimRay, out RaycastHit hitinfo, 50, ~mask))
        {
            target.position = hitinfo.point;
        }
        else
        {
            target.position = cam.position + cam.forward * 50;
        }
    }
}
