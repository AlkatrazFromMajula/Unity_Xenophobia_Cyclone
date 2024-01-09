using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DronePhysiksManager : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;
    [SerializeField] float velocityMultiplier;
    [SerializeField] Transform orientation;
    [SerializeField] Transform cam;
    [SerializeField] float rotationSpeed;
    [SerializeField] float maxRotation;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    float velocityZ;
    float velocityX;

    bool lighteningStrike;

    public bool GetLighteningStrike { get { return lighteningStrike; } }
    public bool LighteningStrike { set { lighteningStrike = value; } }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void HandleCameraRelativeMovement()
    {
        Vector3 viewDir = transform.position -new Vector3(cam.position.x, transform.position.y, cam.position.z);
        orientation.forward = viewDir;

        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 || Input.GetKey(KeyCode.E) != false || Input.GetKey(KeyCode.Q) != false)
        {
            transform.forward = Vector3.Slerp(transform.forward, orientation.forward, rotationSpeed);
        }
    }

    void HandleAccelerationAndDeceleration(bool forwardPressed, bool rightPressed, bool leftPressed, bool backPressed)
    {
        // handle acceleration
        if (forwardPressed && velocityZ <= 1)
        {
            velocityZ += Time.deltaTime * acceleration;
        }
        if (rightPressed && velocityX <= 1)
        {
            velocityX += Time.deltaTime * acceleration;
        }
        if (leftPressed && velocityX >= -1)
        {
            velocityX -= Time.deltaTime * acceleration;
        }
        if (backPressed && velocityZ >= -1)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }

        // handle deceleration
        if (!forwardPressed && velocityZ > 0)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }
        if (!rightPressed && velocityX > 0)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
        if (!leftPressed && velocityX < 0)
        {
            velocityX += Time.deltaTime * deceleration;
        }
        if (!backPressed && velocityZ < 0)
        {
            velocityZ += Time.deltaTime * deceleration;
        }

        // lock or reset velocity
        if (velocityZ > 1)
        {
            velocityZ = 1;
        }
        else if (velocityZ < -1)
        {
            velocityZ = -1;
        }
        if (velocityX > 1)
        {
            velocityX = 1;
        }
        else if (velocityX < -1)
        {
            velocityX = -1;
        }

        if (!forwardPressed && !backPressed && velocityZ != 0 &&
            (velocityZ < 0.1f && velocityZ > -0.1f))
        {
            velocityZ = 0;
        }
        if (!rightPressed && !leftPressed & velocityX != 0 &&
            (velocityX < 0.1f && velocityX > -0.1f))
        {
            velocityX = 0;
        }
    }

    void HndlePitchAndRoll()
    {
        float angleAboutZ = velocityX * maxRotation;
        float angleAboutX = velocityZ * maxRotation;
        Vector3 newRotation = new Vector3(angleAboutX, transform.eulerAngles.y, -angleAboutZ);
        transform.eulerAngles = new Vector3(angleAboutX, transform.eulerAngles.y, -angleAboutZ);
    }

    float ClampRotation(float angle, float min, float max)
    {
        if (angle > max) { return max; }
        else if (angle < min) { return min; }
        else { return angle; }
    }

    void HandleVelocity()
    {
        Vector3 moveDir = orientation.forward * velocityZ + orientation.right * velocityX;
        rb.AddForce(moveDir * 10 * velocityMultiplier, ForceMode.Force);

        HandleLighteningStrike();

        if (Input.GetKey(KeyCode.E))
        {
            rb.AddForce(Vector3.up * 8 * velocityMultiplier, ForceMode.Force);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            rb.AddForce(Vector3.down * 8 * velocityMultiplier, ForceMode.Force);
        }
    }

    void HandleLighteningStrike()
    {
        Vector3 moveDir = orientation.forward * -1;
        if (lighteningStrike)
        {
            rb.AddForce(moveDir * 2 * velocityMultiplier, ForceMode.Impulse);
            lighteningStrike = false;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool backPressed = Input.GetKey(KeyCode.S);

        HandleAccelerationAndDeceleration(forwardPressed, rightPressed, leftPressed, backPressed);
        HandleVelocity();
        HandleCameraRelativeMovement();
        HndlePitchAndRoll();
    }

    private void OnEnable()
    {
        velocityZ = 0;
        velocityX = 0;
    }
}
