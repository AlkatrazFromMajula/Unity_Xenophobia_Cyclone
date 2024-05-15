using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class LighteningShotActivator : MonoBehaviour
{
    [SerializeField] GameObject lightening;
    [SerializeField] GameObject preLightening;
    bool active;

    void OnEnable()
    {
        active = false;
        lightening.SetActive(false);
        preLightening.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !active)
        {
            active = true;
            preLightening.SetActive(true);
            preLightening.GetComponentInChildren<ParticleSystem>().Play();
            lightening.SetActive(true);
        }
        else if (active && !lightening.activeSelf && !preLightening.activeSelf)
        {
            active = false;
        }
    }
}
