using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SpiderFootIK : MonoBehaviour
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
    private float maxHitDistance = 1.5f;
    private float addedHight = 0.5f;
    private bool[] allGroundSpherecastHits;
    private LayerMask hitLayer;
    private Vector3[] allHitNormals;
    private float angleAboutZ;
    private float angleAboutX;
    private float yOffset = 0.015f;
    Animator animator;
    private float[] allFootWeights;
    private Vector3 averageHitNormal;
    [SerializeField, Range(0.0f, 0.2f)] private float upperColliderLimitY = 0.15f;
    [SerializeField, Range(0.0f, 0.2f)] private float lowerColliderLimitY = 0.12f;
    private int[] checkLocalTargetY;

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
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RotateCycloneFeet();
    }

    private void CheckGroundBelow(out Vector3 hitPoint, out bool gotGroundSpherecastHit,
        out Vector3 hitNormal, out LayerMask hitLayer, out float currentHitDistance,
        Transform objectTransform, int checkForLayerMask, float maxHitDistance, float addedHeight)
    {
        RaycastHit hit;
        Vector3 startSpherecast = objectTransform.position + objectTransform.up * addedHeight;

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
            if (Physics.SphereCast(startSpherecast, 0.06f, objectTransform.up * (-1), out hit, maxHitDistance,
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
}