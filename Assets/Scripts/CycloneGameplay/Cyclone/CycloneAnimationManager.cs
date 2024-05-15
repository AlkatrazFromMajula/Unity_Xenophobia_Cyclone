using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CycloneAnimationManager : MonoBehaviour
{
    // declare reference variables
    Animator animator;
    public float velocityZ = 0.0f;
    float velocityX = 0.0f;
    bool crouchPressed;
    bool onLowerEdge;
    bool onUpperEdge;
    bool turnRight;
    bool turnLeft;
    bool usingCharacterTool;


    // populate variables in the inspector
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float maxWalkVelocity;
    [SerializeField] float maxInPlaceTurnVelocity;
    [SerializeField] float maxTurnVelocity;
    [SerializeField] float maxRunVelocity;
    [SerializeField] GameObject freeLookCamera;
    const float zeroRotAndTrans = 0.0f;
    float defaultWalkVelocity;
    Vector2 pointOfInterestTurnVector;
    public bool check;

    // increase performance
    int velocityZHash;
    int velocityXHash;
    int isCrouchingHash;
    int isJumpingHash;
    int onLowerEdgeHash;
    int onUpperEdgeHash;
    int turnRightHash;
    int turnLeftHash;
    int isRunningHash;

    public bool UsingCharacterTool
    {
        set { usingCharacterTool = value; }
    }

    void Awake()
    {
        defaultWalkVelocity = maxWalkVelocity;
    }

    void Start()
    {
        // initially set reference variables
        animator = GetComponent<Animator>();
        velocityZHash = Animator.StringToHash("VelocityZ");
        velocityXHash = Animator.StringToHash("VelocityX");
        isCrouchingHash = Animator.StringToHash("isCrouching");
        isJumpingHash = Animator.StringToHash("isJumping");
        onLowerEdgeHash = Animator.StringToHash("onLowerEdge");
        onUpperEdgeHash = Animator.StringToHash("onUpperEdge");
        turnRightHash = Animator.StringToHash("turnRight");
        turnLeftHash = Animator.StringToHash("turnLeft");
        isRunningHash = Animator.StringToHash("isRunning");
        crouchPressed = false;

    }

    // Handle acceleration
    void HandleAcceleration (bool forwardPressed, bool leftPressed,
        bool rightPressed, bool runPressed, float currentMaxVelocity)
    {
        // calculate forward movement
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }

        // calculate left and right movement (also while running)
        if (rightPressed && forwardPressed && !runPressed && velocityX < maxTurnVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
            if (maxWalkVelocity > maxTurnVelocity)
            {
                maxWalkVelocity = maxTurnVelocity;
            }
        }
        if (rightPressed && forwardPressed && runPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }
        if (leftPressed && forwardPressed && !runPressed && velocityX > -maxTurnVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
            if (maxWalkVelocity > maxTurnVelocity)
            {
                maxWalkVelocity = maxTurnVelocity;
            }
        }
        if (leftPressed && forwardPressed && runPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        // calculate left and right turn
        if (rightPressed && velocityX < maxInPlaceTurnVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
            if (velocityX < maxInPlaceTurnVelocity && velocityX > (maxInPlaceTurnVelocity - 0.05f))
            {
                velocityX = maxInPlaceTurnVelocity;
            }
        }
        if (leftPressed && velocityX > -maxInPlaceTurnVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
            if (velocityX > -maxInPlaceTurnVelocity && velocityX < (-maxInPlaceTurnVelocity + 0.05f))
            {
                velocityX = -maxInPlaceTurnVelocity;
            }
        }
    }

    void HandleDeceleration(bool forwardPressed, bool leftPressed,
        bool rightPressed, bool runPressed, float currentMaxVelocity)
    {
        // calculate deceleration
        if (!forwardPressed && velocityZ > zeroRotAndTrans)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }
        if (!leftPressed && !forwardPressed && velocityX < zeroRotAndTrans)
        {
            velocityX += Time.deltaTime * deceleration;
        }
        if (!rightPressed && !forwardPressed && velocityX > zeroRotAndTrans)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
        if (!forwardPressed && rightPressed && velocityX > maxInPlaceTurnVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
        if (!forwardPressed && leftPressed && velocityX < -maxInPlaceTurnVelocity)
        {
            velocityX += Time.deltaTime * deceleration;
        }
        if (forwardPressed && !rightPressed && velocityX > zeroRotAndTrans)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
        if (forwardPressed && !leftPressed && velocityX < -zeroRotAndTrans)
        {
            velocityX += Time.deltaTime * deceleration;
        }
    }

    void LockOrRecetVelocity(bool forwardPressed, bool leftPressed,
        bool rightPressed, bool runPressed, float currentMaxVelocity)
    {
        if (CyclonePhysicsManager.MustDecelerate && velocityZ > maxWalkVelocity)
        {
            currentMaxVelocity = maxWalkVelocity;
        }
        else if (CyclonePhysicsManager.MustStop && velocityZ > zeroRotAndTrans)
        {
            currentMaxVelocity = zeroRotAndTrans;
        }

        if (!forwardPressed && velocityZ < zeroRotAndTrans)
        {
            velocityZ = zeroRotAndTrans;
        }

        if (!rightPressed && !leftPressed && maxWalkVelocity < defaultWalkVelocity)
        {
            maxWalkVelocity = defaultWalkVelocity;
        }

        if (!leftPressed && !rightPressed && velocityX != zeroRotAndTrans &&
            (velocityX > -0.05f && velocityX < 0.05f))
        {
            velocityX = zeroRotAndTrans;
        }

        //debug stupid head turning while accelerating or decelerating
        if (forwardPressed && rightPressed && velocityX > 0.5f && velocityX < 1.0f)
        {
            velocityX = 1.0f;
        }
        if (forwardPressed && leftPressed && velocityX < -0.5f && velocityX > -1.0f)
        {
            velocityX = -1.0f;
        }

        if (!forwardPressed && !rightPressed && velocityZ > 0 && velocityX > 0.5f && velocityX < 1.0f)
        {
            velocityX = 0.5f;
        }
        if (!forwardPressed && !leftPressed && velocityZ > 0 && velocityX < -0.5f && velocityX > -1.0f)
        {
            velocityX = -0.5f;
        }

        // calculate running forward
        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05))
            {
                velocityZ = currentMaxVelocity;
            }
        }
        else if (forwardPressed && velocityZ < currentMaxVelocity &&
            velocityZ > (currentMaxVelocity - 0.05f))
        {
            velocityZ = currentMaxVelocity;
        }



        // calculate running right
        if (rightPressed && runPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        else if (rightPressed && forwardPressed && velocityX > currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
            if (velocityX > currentMaxVelocity && velocityX < (currentMaxVelocity + 0.05))
            {
                velocityX = currentMaxVelocity;
            }
        }
        else if (rightPressed && forwardPressed && velocityX < currentMaxVelocity &&
            velocityX > (currentMaxVelocity - 0.05f))
        {
            velocityX = currentMaxVelocity;
        }



        // calculate running left
        if (leftPressed && runPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
        else if (leftPressed && forwardPressed && velocityX < -currentMaxVelocity)
        {
            velocityX += Time.deltaTime * deceleration;
            if (velocityX < -currentMaxVelocity && velocityX > (-currentMaxVelocity - 0.05))
            {
                velocityX = -currentMaxVelocity;
            }
        }
        else if (leftPressed && forwardPressed && velocityX > -currentMaxVelocity &&
            velocityX < (-currentMaxVelocity + 0.05f))
        {
            velocityX = -currentMaxVelocity;
        }
    }
    void HandleCrouching(bool forwardPressed, bool leftPressed,
        bool rightPressed, float currentMaxVelocity)
    {
        // calculate forward movement
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }

        // calculate side movement
        if (forwardPressed && rightPressed && velocityX < maxTurnVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }
        if(forwardPressed && leftPressed && velocityX > -maxTurnVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        // calculate in place turn
        if (!forwardPressed && rightPressed && velocityX < maxInPlaceTurnVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }
        if (!forwardPressed && leftPressed && velocityX > -maxInPlaceTurnVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        // calculate deceleration
        if (!forwardPressed && velocityZ > zeroRotAndTrans)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }
        if (!rightPressed && velocityX > zeroRotAndTrans)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
        if (!leftPressed && velocityX < zeroRotAndTrans)
        {
            velocityX += Time.deltaTime * deceleration;
        }
        if (!forwardPressed && velocityZ < zeroRotAndTrans)
        {
            velocityZ = zeroRotAndTrans;
        }
        if (!leftPressed && !rightPressed && velocityX != zeroRotAndTrans &&
            (velocityX > -0.05f && velocityX < 0.05f))
        {
            velocityX = zeroRotAndTrans;
        }

        // lock velocity
        if(forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        if(rightPressed && forwardPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        if (leftPressed && forwardPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
        if(rightPressed && !forwardPressed && velocityX > maxInPlaceTurnVelocity)
        {
            velocityX = maxInPlaceTurnVelocity;
        }
        if (leftPressed && !forwardPressed && velocityX < -maxInPlaceTurnVelocity)
        {
            velocityX = -maxInPlaceTurnVelocity;
        }
    }

    void HandleEmergencyStop()
    {
        // calculate deceleration
        if (velocityZ > zeroRotAndTrans)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }
        if (velocityZ < zeroRotAndTrans)
        {
            velocityZ += Time.deltaTime * deceleration;
        }
        if (velocityX > zeroRotAndTrans)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
        if (velocityX < zeroRotAndTrans)
        {
            velocityX += Time.deltaTime * deceleration;
        }
        if (velocityX != zeroRotAndTrans &&
           velocityX > -0.05f && velocityX < 0.05f)
        {
            velocityX = zeroRotAndTrans;
        }
        if (velocityZ != zeroRotAndTrans &&
           velocityZ > -0.05f && velocityZ < 0.05f)
        {
            velocityZ = zeroRotAndTrans;
        }
    }

    void HandlePointOfInterestTurn(Vector2 pointOfInterestTurnVector)
    {
        if (pointOfInterestTurnVector != Vector2.zero)
        {
            Vector2 currentTurnVector = new Vector2(
                transform.forward.normalized.x, transform.forward.normalized.z);
            if (currentTurnVector != pointOfInterestTurnVector)
            {
                float turnAngle = Vector2.SignedAngle(currentTurnVector, pointOfInterestTurnVector);
                if (turnAngle > 0)
                {
                    turnLeft = true;
                    turnRight = false;
                }
                else if (turnAngle < 0)
                {
                    turnRight = true;
                    turnLeft = false;
                }
            }
            else
            {
                turnRight = false;
                turnLeft = false;
            }
        }
        else
        {
            turnRight = false;
            turnLeft = false;
        }
    }

    void OnTriggerStay(Collider other)
    {

        if (other.CompareTag("Lower Edge") && freeLookCamera.GetComponent<CameraPositionChecker>().IsLookingUP)
        {
            // handle the lower edge jump
            onLowerEdge = true;
            pointOfInterestTurnVector = other.GetComponent<LowerEdge>().NormalizedVector;

        }
        else if (other.CompareTag("Upper Edge") && !freeLookCamera.GetComponent<CameraPositionChecker>().IsLookingUP)
        {
            // handle the upper edge fall
            onUpperEdge = true;
            pointOfInterestTurnVector = other.GetComponent<UpperEdge>().NormalizedVector;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Lower Edge"))
        {
            onLowerEdge = false;
            pointOfInterestTurnVector = Vector2.zero;
        }
        else if (other.CompareTag("Upper Edge"))
        {
            onUpperEdge = false;
            pointOfInterestTurnVector = Vector2.zero;
        }
    }
    void Update()
    {
        // variables for fanaging simple movement
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);

        // manage crouching
        if (!usingCharacterTool)
        {
            if ((Input.GetKeyDown(KeyCode.LeftControl) ||
            (crouchPressed && Input.GetKeyDown(KeyCode.LeftShift))) && CyclonePhysicsManager.MayStandUp)
            {
                crouchPressed = !crouchPressed;
            }
        }

        // handle simple movement
        if (!crouchPressed)
        {
            float currentMaxVelocity = runPressed ? maxRunVelocity : maxWalkVelocity;

            if (!usingCharacterTool)
            {
                HandleAcceleration(forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
                LockOrRecetVelocity(forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
                HandleDeceleration(forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
            }
            else
            {
                HandleEmergencyStop();
            }
        }

        // handle crouching
        else if (crouchPressed)
        {
            if (!usingCharacterTool)
            {
                float currentMaxVelocity = maxWalkVelocity;
                HandleCrouching(forwardPressed, leftPressed, rightPressed, currentMaxVelocity);
            }
        }

        // handle point of interest turn
        HandlePointOfInterestTurn(pointOfInterestTurnVector);

        // set animator parameters
        animator.SetFloat(velocityZHash, velocityZ);
        animator.SetFloat(velocityXHash, velocityX);
        animator.SetBool(isCrouchingHash, crouchPressed);
        animator.SetBool(isJumpingHash, jumpPressed);
        animator.SetBool(onLowerEdgeHash, onLowerEdge);
        animator.SetBool(onUpperEdgeHash, onUpperEdge);
        animator.SetBool(turnRightHash, turnRight);
        animator.SetBool(turnLeftHash, turnLeft);
        animator.SetBool(isRunningHash, runPressed);

        check = usingCharacterTool;
    }

    
}
