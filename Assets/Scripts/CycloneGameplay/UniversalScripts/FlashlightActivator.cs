using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightActivator : MonoBehaviour
{
    [SerializeField] Material lightON;
    [SerializeField] Material lightOFF;
    [SerializeField] GameObject bulb;
    [SerializeField] GameObject beam;
    protected bool active;

    virtual protected void OnEnable()
    {
        bulb.GetComponent<Renderer>().material = lightOFF;
        beam.SetActive(false);
        active = false;
    }

    virtual protected void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            active = !active;

            if (active)
            {
                bulb.GetComponent<Renderer>().material = lightON;
                beam.SetActive(true);
            }
            else
            {
                bulb.GetComponent<Renderer>().material = lightOFF;
                beam.SetActive(false);
            }
        }
    }
}
