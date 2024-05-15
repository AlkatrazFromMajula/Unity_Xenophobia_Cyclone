using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SpiderActivationManager : MonoBehaviour
{
    Ray groundChecker;
    SpiderImmobilizer immobilizer;
    [SerializeField] Collider landTrigger;
    public bool hasLanded;
    [SerializeField] float gravityMultiplier;
    [SerializeField] GameObject cam;
    [SerializeField] CinemachineBrain cinBrain;
    [SerializeField] GameObject cyclone;

    private void Start()
    {
        immobilizer = GetComponent<SpiderImmobilizer>();
    }

    private void Update()
    {
        groundChecker = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(groundChecker, out RaycastHit hitInfo, 3f) && !hasLanded)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.down * gravityMultiplier * 1000 * Time.deltaTime, ForceMode.Force);
            print(hitInfo.distance);
            transform.forward = Vector3.Slerp(transform.forward, cyclone.transform.forward, 15 * Time.deltaTime);
        }
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle-Walk"))
        {
            GetComponent<SpiderController>().enabled = true;
            landTrigger.enabled = false;
        }

        if (Input.GetKeyUp(KeyCode.F) && !immobilizer.QuitProhibited)
        {
            GetComponent<SpiderController>().enabled = false;
            cam.SetActive(false);
            cinBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        hasLanded = true;
    }

    private void OnEnable()
    {
        cam.SetActive(true);
        hasLanded = false;
        landTrigger.enabled = true;
        cinBrain.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
    }
}
