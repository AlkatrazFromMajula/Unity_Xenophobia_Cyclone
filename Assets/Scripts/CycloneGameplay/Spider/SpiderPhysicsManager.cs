using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UIElements;

public class SpiderPhysicsManager : MonoBehaviour
{
    // set reference variables
    Animator animator;
    SpiderAnimationManager animManager;
    SpiderImmobilizer immobilizer;
    Rigidbody rb;
    float velocityZ;
    float velocityX;

    [SerializeField] float velocityMultiplier;

    [SerializeField] float rotationSpeed;
    [SerializeField] GameObject cam;
    [SerializeField] Transform orientation;
    Vector3 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        animManager = GetComponent<SpiderAnimationManager>();
        immobilizer = GetComponent<SpiderImmobilizer>();
        
    }

    void HandleVelocity(float velocityZ, float velocityX)
    {
        // calculate movement direction
        moveDirection = orientation.forward * animManager.inputZ + orientation.right * animManager.inputX;

        // add force
        rb.AddForce(moveDirection.normalized * velocityMultiplier * Time.deltaTime * 100, ForceMode.Force);
    }

    void LimitVelocity()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity
        if (flatVel.magnitude > velocityMultiplier)
        {
            Vector3 limitedVel = flatVel.normalized * velocityMultiplier;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
        if (moveDirection == Vector3.zero)
        {
            rb.velocity = Vector3.zero;
        }
    }

    void HandleCameraRelatedMovement()
    {
        Vector3 viewDir = transform.position - new Vector3(cam.transform.position.x, transform.position.y, cam.transform.position.z);
        orientation.forward = viewDir;
        orientation.localEulerAngles = new Vector3(0, orientation.localEulerAngles.y, 0);

        transform.forward = Vector3.Slerp(transform.forward, orientation.forward, Time.deltaTime * rotationSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        // handle velocity
        // HandleVelocity(velocityZ, velocityX);
        if (!immobilizer.Immobilized)
        {
            velocityZ = animator.GetFloat("velocityZ");
            velocityX = animator.GetFloat("velocityX");

            HandleCameraRelatedMovement();
        }
     // LimitVelocity();

        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
    }
}
