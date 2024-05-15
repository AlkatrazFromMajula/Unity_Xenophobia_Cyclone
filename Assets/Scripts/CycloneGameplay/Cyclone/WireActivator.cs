using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireActivator : MonoBehaviour
{
    [SerializeField] GameObject wire;

    // Update is called once per frame
    void Update()
    {
        if (!wire.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                wire.SetActive(true);
            }
        }
    }
}
