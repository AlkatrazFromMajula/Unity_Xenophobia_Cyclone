using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAnimationManager : MonoBehaviour
{
    Animator animator;

    bool refuse;
    bool hit;
    bool activate;

    int refuseHash;
    int hitHash;
    int activateHash;

    public bool Activate
    {
        set { activate = value; }
    }

    public bool GetHit { get { return hit; } }
    public bool Hit { set { hit = value; } }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        refuseHash = Animator.StringToHash("refuse");
        hitHash = Animator.StringToHash("hit");
        activateHash = Animator.StringToHash("activate");
    }

    private void OnCollisionStay(Collision collision)
    {
        hit = true;
    }
    
    void ManageDiverceAnimations()
    {
        if (animator.GetCurrentAnimatorStateInfo(1).IsName("Drone_Refuse"))
        {
            refuse = false;
        }
        if (animator.GetCurrentAnimatorStateInfo(1).IsName("Drone_Hit"))
        {
            hit = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        refuse = Input.GetKeyDown(KeyCode.R);
        animator.SetBool(refuseHash, refuse);
        animator.SetBool(hitHash, hit);
        animator.SetBool(activateHash, activate);
        ManageDiverceAnimations();
    }
}
