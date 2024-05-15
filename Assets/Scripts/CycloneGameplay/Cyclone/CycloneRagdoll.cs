using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycloneRagdoll : MonoBehaviour
{
    [SerializeField] GameObject[] bodyParts;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (GameObject part in bodyParts)
        {
            if (part.TryGetComponent<Collider>(out Collider coll))
            {
                if (coll != null)
                {
                    coll.enabled = false;
                }
            }
            if (part.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                if (rb != null)
                {
                    rb.useGravity = false;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
