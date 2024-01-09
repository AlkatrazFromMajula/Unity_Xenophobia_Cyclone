using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireNozzleAim : MonoBehaviour
{
    [SerializeField] GameObject wireSectionEnd;
    [SerializeField] Transform aimTarget;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = wireSectionEnd.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = wireSectionEnd.transform.position;
        transform.LookAt(aimTarget);
        transform.forward = -transform.forward;
    }
}
