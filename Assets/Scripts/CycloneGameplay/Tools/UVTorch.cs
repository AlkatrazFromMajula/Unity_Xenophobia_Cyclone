using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVTorch : MonoBehaviour
{
    [SerializeField] Material torchON;
    [SerializeField] Material torchOFF;
    [SerializeField] GameObject bulb;
    [SerializeField] GameObject beam;
    bool active;

    void OnEnable()
    {
        bulb.GetComponent<Renderer>().material = torchOFF;
        beam.SetActive(false);
        active = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            active = !active;

            if (active)
            {
                bulb.GetComponent<Renderer>().material = torchON;
                beam.SetActive(true);
            }
            else
            {
                bulb.GetComponent<Renderer>().material = torchOFF;
                beam.SetActive(false);
            }
        }
    }
}
