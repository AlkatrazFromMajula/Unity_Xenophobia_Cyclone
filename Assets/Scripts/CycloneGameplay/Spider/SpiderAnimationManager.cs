using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAnimationManager : MonoBehaviour
{
    // set reference variables
    Animator animator;
    Timer faultWiringTimer;
    SpiderImmobilizer immobilizer;
    SpiderInteractive currentSpiderInteractive;
    [SerializeField] GameObject sparks;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] CameraPositionChecker cam;
    [SerializeField] Transform orientation;

    // increase performance
    int velocityZHash;
    int velocityXHash;
    int isPerformingActionHash;
    int isInteractingWithWiringHash;

    public float inputZ;
    public float inputX;
    float velocityZ;
    float velocityX;
    bool isPerformingAction;
    bool isInteractingWithWiring;

    public bool hit;
    public bool interact;
    public bool finished;

    // Start is called before the first frame update
    void Start()
    {
        immobilizer = GetComponent<SpiderImmobilizer>();
        animator = GetComponent<Animator>();
        velocityXHash = Animator.StringToHash("velocityX");
        velocityZHash = Animator.StringToHash("velocityZ");
        isPerformingActionHash = Animator.StringToHash("isPerformingAction");
        isInteractingWithWiringHash = Animator.StringToHash("isInteractingWithWiring");

        inputZ = 0;
        inputX = 0;
        isPerformingAction = false;
        isInteractingWithWiring = false;
        sparks.SetActive(false);
    }

    void HandleVelocity(bool forwardPressed, bool rightPressed, bool leftPressed, bool backPressed)
    {
        // handle acceleration
        if (forwardPressed && inputZ <= 1)
        {
            inputZ += Time.deltaTime * acceleration;
        }
        if (rightPressed && inputX <= 1)
        {
            inputX += Time.deltaTime * acceleration;
        }
        if (leftPressed && inputX >= -1)
        {
            inputX -= Time.deltaTime * acceleration;
        }
        if (backPressed && inputZ >= -1)
        {
            inputZ -= Time.deltaTime * acceleration;
        }

        // handle deceleration
        if (!forwardPressed && inputZ > 0)
        {
            inputZ -= Time.deltaTime * deceleration;
        }
        if (!rightPressed && inputX > 0)
        {
            inputX -= Time.deltaTime * deceleration;
        }
        if (!leftPressed && inputX < 0)
        {
            inputX += Time.deltaTime * deceleration;
        }
        if (!backPressed && inputZ < 0)
        {
            inputZ += Time.deltaTime * deceleration;
        }

        // lock or reset velocity
        if (inputZ > 1)
        {
            inputZ = 1;
        }
        else if (inputZ < -1)
        {
            inputZ = -1;
        }
        if (inputX > 1)
        {
            inputX = 1;
        }
        else if (inputX < -1)
        {
            inputX = -1;
        }

        if (!forwardPressed && !backPressed && inputZ != 0 &&
            (inputZ < 0.1f && inputZ > -0.1f))
        {
            inputZ = 0;
        }
        if (!rightPressed && !leftPressed & inputX != 0 &&
            (inputX < 0.1f && inputX > -0.1f))
        {
            inputX = 0;
        }
    }

    void HandleCameraRelatedAnimation()
    {
        Vector2 animOrientationForward = new Vector2(orientation.forward.z, orientation.forward.x);
        Vector2 animOrientationRight = new Vector2(orientation.right.z, orientation.right.x);
        velocityZ = animOrientationForward.x * inputZ + animOrientationRight.x * inputX;
        velocityX = animOrientationForward.y * inputZ + animOrientationRight.y * inputX;
        if (velocityX > 1)
        {
            velocityX = 1;
        }
        if (velocityX < -1)
        {
            velocityX = -1;
        }
        if (velocityZ > 1)
        {
            velocityZ = 1;
        }
        if (velocityZ < -1)
        {
            velocityZ = -1;
        }
    }

    void HandleFaultWiring()
    {
        Ray wiringDetector = new Ray(transform.position + transform.up * 0.075f, transform.up * -1);
        if (Physics.Raycast(wiringDetector, out RaycastHit hitinfo, 0.15f))
        {
            if (hitinfo.collider.gameObject.layer == 12)
            {
                hit = true;
                currentSpiderInteractive = hitinfo.collider.gameObject.GetComponent<SpiderInteractive>();
                if (Input.GetKeyUp(KeyCode.E) && !isPerformingAction)
                {
                    immobilizer.ProhibitQuit = true;
                    immobilizer.Immobilize = true;
                    interact = true;
                    isInteractingWithWiring = true;
                    isPerformingAction = true;
                    faultWiringTimer = gameObject.AddComponent<Timer>();
                    faultWiringTimer.Duration = 8f;
                    faultWiringTimer.Run();
                }
            }
            else
            {
                hit = false;
            }
        }

        if (faultWiringTimer != null)
        {
            if (!faultWiringTimer.Finished && animator.GetCurrentAnimatorStateInfo(0).IsName("Spider_FaultWiring")
                && (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.18))
            {
                sparks.SetActive(true);
            }
        }

        if (faultWiringTimer != null)
        {
            if (faultWiringTimer.Finished)
            {
                immobilizer.ProhibitQuit = false;
                immobilizer.Immobilize = false;
                isInteractingWithWiring = false;
                isPerformingAction = false;
                sparks.SetActive(false);
                currentSpiderInteractive.InteractionFinished = true;
                currentSpiderInteractive = null;
                Destroy(faultWiringTimer);
                finished = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool backPressed = Input.GetKey(KeyCode.S);

        HandleVelocity(forwardPressed, rightPressed, leftPressed, backPressed);
        HandleCameraRelatedAnimation();
        HandleFaultWiring();

        if (!immobilizer.Immobilized)
        {
            animator.SetFloat(velocityZHash, inputZ);
            animator.SetFloat(velocityXHash, inputX);
        }
        isPerformingAction = immobilizer.Immobilized;
        animator.SetBool(isPerformingActionHash, isPerformingAction);
        animator.SetBool(isInteractingWithWiringHash, isInteractingWithWiring);
    }
}
