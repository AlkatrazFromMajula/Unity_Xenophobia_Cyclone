using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LighteningRaycast : MonoBehaviour
{
    [SerializeField] Transform aimTarget;

    bool shoot;

    public bool Shoot { set { shoot = value; } }

    LayerMask cycloneMask;

    private void Awake()
    {
        cycloneMask = LayerMask.GetMask("Cyclone");
    }

    // Update is called once per frame
    void Update()
    {
        if (shoot)
        {
            shoot = false;

            if (Physics.Raycast(transform.position, aimTarget.position - transform.position, out RaycastHit hitinfo, (aimTarget.position - transform.position).magnitude, ~cycloneMask))
            {
                if (hitinfo.collider.gameObject.layer == 13)
                {
                    Debug.Log("YES");
                    hitinfo.collider.gameObject.GetComponent<LighteningInteractive>().InteractionFinished = true;

                }
            }
        }
    }
}
