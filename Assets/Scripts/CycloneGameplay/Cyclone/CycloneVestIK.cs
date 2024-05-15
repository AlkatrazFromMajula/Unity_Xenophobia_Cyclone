using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycloneVestIK : MonoBehaviour
{
    [SerializeField] Transform targetBoneTransform;

    private void LateUpdate()
    {
        transform.position = targetBoneTransform.position;
        transform.rotation = targetBoneTransform.rotation;
    }
}
