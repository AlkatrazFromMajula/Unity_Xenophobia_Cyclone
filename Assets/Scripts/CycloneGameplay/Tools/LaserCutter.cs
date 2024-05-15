using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCutter : MonoBehaviour
{
    [SerializeField] GameObject laserBeam;
    [SerializeField] GameObject laserSparks;
    [SerializeField] GameObject laserSparksHit;
    float laserLength;

    // Start is called before the first frame update
    void Start()
    {
        laserLength = 0;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(transform.position, transform.up, out RaycastHit hitInfo, 50))
            {
                laserLength = hitInfo.distance;
                laserBeam.SetActive(true);
                laserSparks.transform.parent = null;
                laserSparks.transform.position = hitInfo.point;
                laserSparks.transform.forward = hitInfo.normal;
                laserSparks.SetActive(true);

                if (hitInfo.collider.gameObject.layer == 11)
                {
                    hitInfo.collider.gameObject.GetComponent<LaserCutterInteractive>().TouchedByLaser = true;
                }
            }
            else
            {
                laserLength = 0;
                laserBeam.SetActive(false);
                laserSparks.SetActive(false);
                laserSparks.transform.parent = transform;
                laserSparks.transform.localPosition = Vector3.zero;
            }
        }
        else
        {
            laserLength = 0;
            laserBeam.SetActive(false);
            laserSparks.SetActive(false);
            laserSparks.transform.parent = transform;
            laserSparks.transform.localPosition = Vector3.zero;
        }

        laserBeam.transform.GetChild(0).localScale = new Vector3(laserBeam.transform.GetChild(0).localScale.x, laserBeam.transform.GetChild(0).localScale.y, laserLength / 10);
    }
}
