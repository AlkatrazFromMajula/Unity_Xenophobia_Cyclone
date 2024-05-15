using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LighteningCube : MonoBehaviour
{
    [SerializeField] Material lightMat;

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<LighteningInteractive>().InteractionSuccess)
        {
            gameObject.GetComponent<Renderer>().material = lightMat;
        }

    }
}
