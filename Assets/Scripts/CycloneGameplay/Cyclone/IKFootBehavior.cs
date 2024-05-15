using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class IKFootBehavior : MonoBehaviour
{
    [SerializeField] private Transform footTransformRF;
    [SerializeField] private Transform footTransformLF;
    [SerializeField] private Transform footTransformRB;
    [SerializeField] private Transform footTransformLB;
    private Transform[] allFootTransforms;
    [SerializeField] private Transform footTargetTransformRF;
    [SerializeField] private Transform footTargetTransformLF;
    [SerializeField] private Transform footTargetTransformRB;
    [SerializeField] private Transform footTargetTransformLB;
    private Transform[] allTargetTransforms;
    [SerializeField] private GameObject footRigRF;
    [SerializeField] private GameObject footRigLF;
    [SerializeField] private GameObject footRigRB;
    [SerializeField] private GameObject footRigLB;
    private TwoBoneIKConstraint[] allFootIKConstraints;
    private LayerMask groundLayerMask;
    private float maxHitDistance = 5f;
    private float addedHight = 1f;
    private bool[] allGroundSpherecastHits;
    private LayerMask hitLayer;
    private Vector3[] allHitNormals;
    private float angleAboutZ;
    private float angleAboutX;
    private float yOffset = 0.15f;
    Animator animator;
    private float[] allFootWeights;
    private Vector3 averageHitNormal;
    [SerializeField, Range(0.0f, 2f)] private float upperColliderLimitY = 1.5f;
    [SerializeField, Range(0.0f, 2f)] private float lowerColliderLimitY = 1.2f;
    private int[] checkLocalTargetY;
    private CharacterController charCon;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        allFootTransforms = new Transform[4];
        allFootTransforms[0] = footTransformRF;
        allFootTransforms[1] = footTransformLF;
        allFootTransforms[2] = footTransformRB;
        allFootTransforms[3] = footTransformLB;

        allTargetTransforms = new Transform[4];
        allTargetTransforms[0] = footTargetTransformRF;
        allTargetTransforms[1] = footTargetTransformLF;
        allTargetTransforms[2] = footTargetTransformRB;
        allTargetTransforms[3] = footTargetTransformLB;

        allFootIKConstraints = new TwoBoneIKConstraint[4];
        allFootIKConstraints[0] = footRigRF.GetComponent<TwoBoneIKConstraint>();
        allFootIKConstraints[1] = footRigLF.GetComponent<TwoBoneIKConstraint>();
        allFootIKConstraints[2] = footRigRB.GetComponent<TwoBoneIKConstraint>();
        allFootIKConstraints[3] = footRigLB.GetComponent<TwoBoneIKConstraint>();

        groundLayerMask = LayerMask.NameToLayer("Ground");

        allGroundSpherecastHits = new bool[5];

        allHitNormals = new Vector3[4];

        allFootWeights = new float[4];

        checkLocalTargetY = new int[4];

        charCon = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RotateCycloneFeet();
        RotateCycloneBody();
        ColliderHeightAdjustments();
    }

    private void CheckGroundBelow(out Vector3 hitPoint, out bool gotGroundSpherecastHit,
        out Vector3 hitNormal, out LayerMask hitLayer, out float currentHitDistance,
        Transform objectTransform, int checkForLayerMask, float maxHitDistance, float addedHeight)
    {
        RaycastHit hit;
        Vector3 startSpherecast = objectTransform.position + new Vector3(0f, addedHeight, 0f);

        if (checkForLayerMask == -1)
        {
            Debug.Log("Layer does not exist");
            gotGroundSpherecastHit = false;
            currentHitDistance = 0f;
            hitLayer = LayerMask.NameToLayer("Cyclone");
            hitNormal = Vector3.up;
            hitPoint = objectTransform.position;

        }
        else 
        {
            int layerMask = (1 << checkForLayerMask);
            if (Physics.SphereCast(startSpherecast, 0.6f, Vector3.down, out hit, maxHitDistance,
                layerMask, QueryTriggerInteraction.UseGlobal))
            {
                hitLayer = hit.transform.gameObject.layer;
                currentHitDistance = hit.distance - addedHeight;
                hitNormal = hit.normal;
                gotGroundSpherecastHit = true;
                hitPoint = hit.point;
            }
            else
            {
                Debug.Log("Layer does not exist");
                gotGroundSpherecastHit = false;
                currentHitDistance = 0f;
                hitLayer = LayerMask.NameToLayer("Cyclone");
                hitNormal = Vector3.up;
                hitPoint = objectTransform.position;
            } 
        }
    }

    Vector3 ProjectOnContactPlane(Vector3 vector, Vector3 hitNormal)
    {
        return vector - hitNormal * Vector3.Dot(vector, hitNormal);
    }

    private void ProjectedAxisAngles(out float angleAboutX, out float angleAboutZ,
        Transform footTargetTransform, Vector3 hitNormal)
    {
        Vector3 xAxisProjected = ProjectOnContactPlane(footTargetTransform.forward, hitNormal).normalized;
        Vector3 zAxisProjected = ProjectOnContactPlane(footTargetTransform.right, hitNormal).normalized;

        angleAboutX = Vector3.SignedAngle(footTargetTransform.forward,
            xAxisProjected, footTargetTransform.right);
        angleAboutZ = Vector3.SignedAngle(footTargetTransform.right,
            zAxisProjected, footTargetTransform.forward);

    }

    private void RotateCycloneFeet()
    {
        allFootWeights[0] = animator.GetFloat("RF Foot Weight");
        allFootWeights[1] = animator.GetFloat("LF Foot Weight");
        allFootWeights[2] = animator.GetFloat("RB Foot Weight");
        allFootWeights[3] = animator.GetFloat("LB Foot Weight");

        for (int i = 0; i < 4; i++)
        {
            allFootIKConstraints[i].weight = allFootWeights[i];

            CheckGroundBelow(out Vector3 hitPoint, out allGroundSpherecastHits[i],
                out Vector3 hitNormal, out hitLayer, out _, allFootTransforms[i],
                groundLayerMask, maxHitDistance, addedHight);
            allHitNormals[i] = hitNormal;

            if (allGroundSpherecastHits[i] == true)
            {
                ProjectedAxisAngles(out angleAboutX, out angleAboutZ,
                    allFootTransforms[i], allHitNormals[i]);

                allTargetTransforms[i].position = new Vector3(allFootTransforms[i].position.x,
                    hitPoint.y + yOffset, allFootTransforms[i].position.z);

                allTargetTransforms[i].rotation = allFootTransforms[i].rotation;

                //allTargetTransforms[i].localEulerAngles = new Vector3(
                //    allTargetTransforms[i].localEulerAngles.x + angleAboutX,
                //    allTargetTransforms[i].localEulerAngles.y,
                //    allTargetTransforms[i].localEulerAngles.z + angleAboutZ);
            }
            else
            {
                allTargetTransforms[i].position = allFootTransforms[i].position;
                allTargetTransforms[i].rotation = allFootTransforms[i].rotation;
            }

        }
    }

    private void RotateCycloneBody()
    {
        float maxRotationStep = 1.0f;
        float averageHitNormalX = 0f;
        float averageHitNormalY = 0f;
        float averageHitNormalZ = 0f;
        for (int i = 0; i < 4; i++)
        {
            averageHitNormalX += allHitNormals[i].x;
            averageHitNormalY += allHitNormals[i].y;
            averageHitNormalZ += allHitNormals[i].z;
        }
        averageHitNormal = new Vector3(averageHitNormalX / 4,
            averageHitNormalY / 4, averageHitNormalZ / 4).normalized;

        ProjectedAxisAngles(out angleAboutX, out angleAboutZ, transform, averageHitNormal);

        float maxRotationX = 30;
        float maxRotationZ = 30;

        float cycloneRotationX = transform.eulerAngles.x;
        float cycloneRotationZ = transform.eulerAngles.z;

        if (cycloneRotationX > 180)
        {
            cycloneRotationX -= 360;
        }
        if (cycloneRotationZ > 180)
        {
            cycloneRotationZ -= 360;
        }

        if (cycloneRotationX + angleAboutX < -maxRotationX)
        {
            angleAboutX = maxRotationX + cycloneRotationX;
        }
        else if (cycloneRotationX + angleAboutX > maxRotationX)
        {
            angleAboutX = maxRotationX - cycloneRotationX;
        }
        if (cycloneRotationZ + angleAboutZ < -maxRotationZ)
        {
            angleAboutZ = maxRotationZ + cycloneRotationZ;
        }
        else if (cycloneRotationZ + angleAboutZ > maxRotationZ)
        {
            angleAboutZ = maxRotationZ - cycloneRotationZ;
        }

        float bodyEulerX = Mathf.MoveTowardsAngle(0, angleAboutX, maxRotationStep);
        float bodyEulerZ = Mathf.MoveTowardsAngle(0, angleAboutZ, maxRotationStep);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x + bodyEulerX,
            transform.eulerAngles.y, transform.eulerAngles.z + bodyEulerZ);
    }

    private void ColliderHeightAdjustments()
    {
        for (int i = 0; i < 4; i++)
        {
            if (allTargetTransforms[i].localPosition.y < allFootTransforms[i].localPosition.y)
            {
                checkLocalTargetY[i] = 1;
            }
            else
            {
                checkLocalTargetY[i] = -1;
            }
        }
        
        if (checkLocalTargetY[0] == 1 && checkLocalTargetY[1] == 1 ||
            checkLocalTargetY[2] == 1 && checkLocalTargetY[3] == 1)
        {
            if(charCon.center.y < upperColliderLimitY)
            {
                charCon.center += new Vector3(0.0f, 0.0025f, 0.0f);
            }
        }
        else if (charCon.center.y > lowerColliderLimitY)
        {
            charCon.center -= new Vector3(0.0f, 0.001f, 0.0f);
        }
        else
        {
            charCon.center = new Vector3(0.0f, 0.12f, 0.0f);
        }
    }
}
