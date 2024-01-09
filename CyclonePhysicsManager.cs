using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CyclonePhysicsManager : MonoBehaviour
{
    // variable to store character controller component and animator component
    CharacterController charCon;
    Animator animator;

    // variables to store rays
    Ray jumpRay;
    Ray fallRay;
    Ray headCollisionRayUp;
    Ray headCollisionRayForward;
    Ray headCollisionRayRight45;
    Ray headCollisionRayRight90;
    Ray headCollisionRayLeft45;
    Ray headCollisionRayLeft90;
    Ray backCollisionRayRight;
    Ray backCollisionRayLeft;

    // variables to store layerMasks
    int edgeWallLayerMask;

    // populated in the inspector for efficiency
    [SerializeField] bool isGrounded;
    [SerializeField] bool isFalling;
    [SerializeField] float constantGravity;
    [SerializeField] Transform headTransform;
    [SerializeField] Transform backTransform;
    float fallHight;
    float switchAnimationState;
    float minFallDistance;
    float minCountinueJumpDistance = 1f;
    float afterJumpSpeed;
    bool mustLand = false;
    Vector3 jumpDistance;
    float edgeJumpSpeedMultiplier;
    float edgeHeight;
    float edgeWidth;
    bool onMovingPaddle;
    static bool isExecutingAJump;
    static bool mayStandUp;
    static bool mustDecelerate;
    static bool mustStop;
    Vector3 movingPaddlePositionTarget;

    // saved for efficiency
    [SerializeField] Vector3 currentMovement;

    // increase performance
    int isGroundedHash;
    int isFallingHash;
    int fallHightHash;
    int mustLandHash;
    int edgeJumpSpeedMultiplierHash;
    int onMovingPaddleHash;

    // priperties
    public static bool IsExecutingAJump
    {
        get { return isExecutingAJump; }
    }

    public static bool MayStandUp
    {
        get { return mayStandUp; }
    }

    public static bool MustDecelerate
    {
        get { return mustDecelerate; }
    }

    public static bool MustStop
    {
        get { return mustStop; }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isGroundedHash = Animator.StringToHash("isGrounded");
        isFallingHash = Animator.StringToHash("isFalling");
        fallHightHash = Animator.StringToHash("fallHight");
        mustLandHash = Animator.StringToHash("mustLand");
        edgeJumpSpeedMultiplierHash = Animator.StringToHash("edgeJumpSpeedMultiplier");
        onMovingPaddleHash = Animator.StringToHash("onMovingPaddle");
        edgeWallLayerMask = 9;
        charCon = GetComponent<CharacterController>();
        currentMovement = new Vector3(0.0f, 0.0f, 0.0f);
    }

    void HandleGravity()
    {
        if (onMovingPaddle && (animator.GetCurrentAnimatorStateInfo(0).IsName("Fix on a Moving Paddle") || animator.GetCurrentAnimatorStateInfo(0).IsName("Stay Fixed")))
        {
            if (transform.position != movingPaddlePositionTarget)
            {
                Vector3 moveToTargetVector = new Vector3(movingPaddlePositionTarget.x - transform.position.x, movingPaddlePositionTarget.y - transform.position.y, movingPaddlePositionTarget.z - transform.position.z);
                currentMovement = moveToTargetVector * 10.0f;
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Long Jump")
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
        {
            isExecutingAJump = true;
            currentMovement.y = 0.0f;
            isGrounded = false;
            minFallDistance = 7f;
            switchAnimationState = 0.6f;
            afterJumpSpeed = 40f;
            jumpRay = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(jumpRay, out RaycastHit hitInfo))
            {
                Debug.DrawLine(transform.position, hitInfo.point, Color.red, 360f);

                // switch to fall animation if too high
                if (hitInfo.distance > minFallDistance &&
                    animator.GetCurrentAnimatorStateInfo(0).normalizedTime > switchAnimationState)
                {
                    isFalling = true;
                }

                // end jump if too low
                else if (hitInfo.distance < minCountinueJumpDistance &&
                    animator.GetCurrentAnimatorStateInfo(0).normalizedTime > switchAnimationState)
                {
                    mustLand = true;
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f)
                {
                    mustLand = true;
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + 100.0f * Time.deltaTime, transform.localEulerAngles.z);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - 100.0f * Time.deltaTime, transform.localEulerAngles.z);
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
        {
            isExecutingAJump = true;
            constantGravity -= animator.GetCurrentAnimatorStateInfo(0).normalizedTime * Time.deltaTime;
            currentMovement = transform.forward * afterJumpSpeed;
            currentMovement.y = constantGravity;
            if (afterJumpSpeed > 0)
            {
                afterJumpSpeed -= Time.deltaTime * 45f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + 100.0f * Time.deltaTime, transform.localEulerAngles.z);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - 100.0f * Time.deltaTime, transform.localEulerAngles.z);
            }

        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Lower Edge Jump"))
        {
            isExecutingAJump = true;
            edgeHeight = Mathf.Abs(jumpDistance.y);
            edgeWidth = Mathf.Sqrt(jumpDistance.sqrMagnitude - Mathf.Pow(jumpDistance.y, 2));
            isGrounded = false;
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.17f)
            {
                currentMovement = new Vector3(0.0f, 0.0f, 0.0f);
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.17f &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.21f)
            {
                currentMovement = transform.forward * edgeWidth * 2f;
                currentMovement.y = 5.0f * edgeHeight;
                edgeJumpSpeedMultiplier = 1.0f;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f &&
                currentMovement.y > constantGravity)
            {
                currentMovement.y -= 15.0f * edgeHeight * Time.deltaTime;
            }
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Upper Edge Preparation"))
        {
            isExecutingAJump = true;
            currentMovement.y = 0.0f;
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Upper Edge Jump"))
        {
            isExecutingAJump = true;
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.75f)
            {
                edgeWidth = Mathf.Abs(jumpDistance.z);
                currentMovement = transform.forward * edgeWidth * 1f;
                currentMovement.y = constantGravity;
            }
        }
        else
        {
            isExecutingAJump = false;
            edgeHeight = 0.0f;
            edgeWidth = 0.0f;
            if (isGrounded)
            {
                mustLand = false;
            }
            edgeJumpSpeedMultiplier = 0.5f;
            constantGravity = -35f;
            currentMovement = new Vector3(0.0f, constantGravity, 0.0f);
        }
    }

    void HandleFalling()
    {
        if (isFalling)
        {
            isFalling = !isGrounded;
            fallRay = new Ray(transform.position, Vector3.down);
            if (Physics.Raycast(fallRay, out RaycastHit hitInfo))
            {
                Debug.DrawLine(transform.position, hitInfo.point, Color.green, 360f);

                // calculate fall hight
                if (hitInfo.distance < 5f)
                {
                    fallHight = hitInfo.distance * 2;
                    if (hitInfo.distance < 0.01f)
                    {
                        fallHight = 0;
                        isFalling = false;
                    }
                }
                else if (hitInfo.distance > 5f && fallHight < 10f)
                {
                    fallHight += Time.deltaTime * 5.0f;
                }
            }
        }
        else
        {
            fallHight = 0;
        }

    }

    void HandleAdditionalCollision()
    {
        headCollisionRayUp = new Ray(headTransform.position, transform.up);
        headCollisionRayForward = new Ray(headTransform.position, transform.forward);
        headCollisionRayRight45 = new Ray(headTransform.position, new Vector3(transform.forward.x + transform.right.x, transform.forward.y + transform.right.y, transform.forward.z + transform.right.z));
        headCollisionRayRight90 = new Ray(headTransform.position, transform.right);
        headCollisionRayLeft45 = new Ray(headTransform.position, new Vector3(transform.forward.x - transform.right.x, transform.forward.y - transform.right.y, transform.forward.z - transform.right.z));
        headCollisionRayLeft90 = new Ray(headTransform.position, new Vector3(-transform.right.x, -transform.right.y, -transform.right.z));
        backCollisionRayRight = new Ray(backTransform.position, new Vector3(-transform.forward.x + transform.right.x, -transform.forward.y + transform.right.y, -transform.forward.z + transform.right.z));
        backCollisionRayLeft = new Ray(backTransform.position, new Vector3(-transform.forward.x - transform.right.x, -transform.forward.y - transform.right.y, -transform.forward.z - transform.right.z));

        mayStandUp = !Physics.Raycast(headCollisionRayUp, 2f, edgeWallLayerMask);

        if (Physics.Raycast(headCollisionRayForward, 2.5f, edgeWallLayerMask) && !Physics.Raycast(headCollisionRayLeft45, 1f, edgeWallLayerMask) && !Physics.Raycast(headCollisionRayRight45, 1f, edgeWallLayerMask))
        {
            mustDecelerate = true;
        }
        else if (Physics.Raycast(headCollisionRayForward, 2f, edgeWallLayerMask) && Physics.Raycast(headCollisionRayLeft45, 1f, edgeWallLayerMask) && Physics.Raycast(headCollisionRayRight45, 1f, edgeWallLayerMask))
        {
            mustStop = true;
        }
        else
        {
            mustStop = false;
            mustDecelerate = false;
        }

            if ((Physics.Raycast(headCollisionRayRight45, 1f, edgeWallLayerMask) && Physics.Raycast(headCollisionRayForward, 1f, edgeWallLayerMask)) || Physics.Raycast(headCollisionRayRight90, 1f, edgeWallLayerMask))
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - 1000.0f * Time.deltaTime, transform.localEulerAngles.z);
        }
        else if ((Physics.Raycast(headCollisionRayLeft45, 1f, edgeWallLayerMask) && Physics.Raycast(headCollisionRayForward, 1f, edgeWallLayerMask)) || Physics.Raycast(headCollisionRayLeft90, 1f, edgeWallLayerMask))
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + 1000.0f * Time.deltaTime, transform.localEulerAngles.z);
        }

        if(Physics.Raycast(backCollisionRayRight, 1f, edgeWallLayerMask))
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + 1000.0f * Time.deltaTime, transform.localEulerAngles.z);
        }
        if (Physics.Raycast(backCollisionRayLeft, 1f, edgeWallLayerMask))
        {
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - 1000.0f * Time.deltaTime, transform.localEulerAngles.z);
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Lower Edge"))
        {
            jumpDistance = other.GetComponent<LowerEdge>().EdgeDistance;
        }
        else if (other.CompareTag("Upper Edge"))
        {
            jumpDistance = other.GetComponent<UpperEdge>().EdgeDistance;
        }
        else if (other.CompareTag("Moving Paddle Target"))
        {
            if (other.transform.parent.gameObject.GetComponent<MovingPaddles>().IsMoving)
            {
                onMovingPaddle = true;
                movingPaddlePositionTarget = other.transform.parent.gameObject.GetComponent<MovingPaddles>().CycloneAimPosition;
            }
            else
            {
                onMovingPaddle = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // call methods
        HandleGravity();
        HandleFalling();
        HandleAdditionalCollision();

        // set the movement
        charCon.Move(currentMovement * Time.deltaTime);

        // update information in the inspector
        isGrounded = charCon.isGrounded;

        // set animator parameters
        animator.SetBool(isGroundedHash, isGrounded);
        animator.SetBool(isFallingHash, isFalling);
        animator.SetFloat(fallHightHash, fallHight);
        animator.SetBool(mustLandHash, mustLand);
        animator.SetFloat(edgeJumpSpeedMultiplierHash, edgeJumpSpeedMultiplier);
        animator.SetBool(onMovingPaddleHash, onMovingPaddle);

    }
}
