using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneActivationManager : MonoBehaviour
{
    [SerializeField] GameObject cam;
    [SerializeField] CinemachineBrain cinBrain;
    [SerializeField] GameObject cyclone;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {

            GetComponent<DronePhysiksManager>().enabled = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            cam.SetActive(false);
            cinBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
            GetComponent<DroneAnimationManager>().Activate = false;
            gameObject.SetActive(false);
        }

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).IsName("Drone_Idle"))
        {
            GetComponent<DronePhysiksManager>().enabled = true;
            cam.SetActive(true);
        }
    }

    private void OnEnable()
    {
        transform.forward = cyclone.transform.forward;
        cinBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
    }
}
