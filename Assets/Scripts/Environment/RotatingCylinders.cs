using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingCylinders : MonoBehaviour
{
    [SerializeField] float rotationSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0.0f, 1.0f * Time.deltaTime * rotationSpeed, 0.0f));
    }
}
