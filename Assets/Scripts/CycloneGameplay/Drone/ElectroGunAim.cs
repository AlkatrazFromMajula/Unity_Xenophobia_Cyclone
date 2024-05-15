using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectroGunAim : MonoBehaviour
{
    [SerializeField] Transform electroGunBase;
    [SerializeField] Transform electroGunForearm;
    [SerializeField] Transform aimTarget;
    [SerializeField] CameraPositionChecker camPosChecker;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //electroGunForearm.localEulerAngles = new Vector3(camPosChecker.AngleX, 0, 0);
        electroGunForearm.LookAt(aimTarget);
        electroGunForearm.localEulerAngles = new Vector3(-electroGunForearm.localEulerAngles.x, 0, 0);
        electroGunBase.localEulerAngles = new Vector3(0, -camPosChecker.AngleZ, 0);

    }
}
