using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraPositionChecker : MonoBehaviour
{
    [SerializeField] Transform target;

    float angleX;
    float angleZ;
    bool isLookingUp;

    public float AngleX { get { return angleX; } }
    public float AngleZ { get { return angleZ; } }
    public bool IsLookingUP { get { return isLookingUp; } }


    // Update is called once per frame
    void Update()
    {
        Vector3 cycloneToCameraVector = new Vector3(transform.position.x - target.position.x,
           transform.position.y - target.position.y,
          transform.position.z - target.position.z);

        angleX = Vector3.SignedAngle(target.up,
            cycloneToCameraVector.normalized, Vector3.Cross(target.up, cycloneToCameraVector.normalized));

        angleZ = Vector2.SignedAngle(new Vector2(target.forward.z * (-1), target.forward.x * (-1)),
            new Vector2(cycloneToCameraVector.normalized.z, cycloneToCameraVector.normalized.x));

        angleX = (angleX - 70);
        angleZ *= (-1);

        if (angleX < 0)
        {
            isLookingUp = false;
        }
        else
        {
            isLookingUp = true;
        }

        //print(angleX);
        //print(angleZ);
    }
}
