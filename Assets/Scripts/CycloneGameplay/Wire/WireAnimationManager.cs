using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireAnimationManager : MonoBehaviour
{
    //declare reference variables
    Animator animator;
    [SerializeField] CameraPositionChecker cameraPositionChecker;

    // increase performance
    int useTorchHash;
    int useCutterHash;
    int useSpiderHash;
    int useDroneHash;
    int useMagnetHash;
    int useBarrierHash;
    int useInjection1Hash;
    int useInjection2Hash;
    int useInjection3Hash;
    int useInjection4Hash;
    int isAimingHash;
    int aimHorizontalHash;
    int aimVerticalHash;

    bool useTorch;
    bool useCutter;
    bool useSpider;
    bool useDrone;
    bool useMagnet;
    bool useBarrier;
    bool useInjection1;
    bool useInjection2;
    bool useInjection3;
    bool useInjection4;
    bool isUsingTool;
    bool isAiming;
    float aimHorizontal;
    float aimVertical;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        useTorchHash = Animator.StringToHash("useTorch");
        useCutterHash = Animator.StringToHash("useCutter");
        useSpiderHash = Animator.StringToHash("useSpider");
        useDroneHash = Animator.StringToHash("useDrone");
        useMagnetHash = Animator.StringToHash("useMagnet");
        useBarrierHash = Animator.StringToHash("useBarrier");
        useInjection1Hash = Animator.StringToHash("useInjection1");
        useInjection2Hash = Animator.StringToHash("useInjection2");
        useInjection3Hash = Animator.StringToHash("useInjection3");
        useInjection4Hash = Animator.StringToHash("useInjection4");
        isAimingHash = Animator.StringToHash("isAiming");
        aimHorizontalHash = Animator.StringToHash("aimHorizontal");
        aimVerticalHash = Animator.StringToHash("aimVertical");
    }

    // Update is called once per frame

    void HandleToolApplication(bool useTorch, bool useCutter, bool useDrone, bool useMagnet, bool useSpider, bool useBarrier, bool useInjection1, bool useInjection2, bool useInjection3, bool useInjection4, bool isUsingTool)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Wire_Prepare_reversed"))
        {
            useTorch = false;
            useCutter = false;
            useSpider = false;
            useDrone = false;
            useMagnet = false;
            useBarrier = false;
            useInjection1 = false;
            useInjection2 = false;
            useInjection3 = false;
            useInjection4 = false;
            isUsingTool = false;

        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Exit State"))
        {
            gameObject.SetActive(false);
        }

        this.useTorch = useTorch;
        this.useCutter = useCutter;
        this.useSpider = useSpider;
        this.useDrone = useDrone;
        this.useMagnet = useMagnet;
        this.useBarrier = useBarrier;
        this.useInjection1 = useInjection1;
        this.useInjection2 = useInjection2;
        this.useInjection3 = useInjection3;
        this.useInjection4 = useInjection4;
        this.isUsingTool = isUsingTool;
    }

    void HandleAiming(bool isAiming, float aimHorizontal, float aimVertical)
    {
        if (isAiming)
        {
            if (aimHorizontal > -175 && aimHorizontal < 175)
            {
                aimHorizontal = cameraPositionChecker.AngleZ;
            }
            if (aimVertical >= -45 && aimVertical <= 45)
            {
                aimVertical = cameraPositionChecker.AngleX;
            }
            else if (aimVertical < -45)
            {
                aimVertical = -45;
            }
            else
            {
                aimVertical = 45;
            }

            if (aimHorizontal >= -175 && aimHorizontal <= 175)
            {
                aimHorizontal = cameraPositionChecker.AngleZ;
            }
            else if (aimHorizontal < -175)
            {
                aimHorizontal = -175;
            }
            else
            {
                aimHorizontal = 175;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                isAiming = false;
            }
        }
        this.isAiming = isAiming;
        this.aimHorizontal = aimHorizontal;
        this.aimVertical = aimVertical;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isUsingTool)
        {
            useTorch = true;
            isUsingTool = true;
            isAiming = true;
            
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && !isUsingTool)
        {
            useCutter = true;
            isUsingTool = true;
            isAiming = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && !isUsingTool)
        {
            useSpider = true;
            isUsingTool = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && !isUsingTool)
        {
            useDrone = true;
            isUsingTool = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5) && !isUsingTool)
        {
            useMagnet = true;
            isUsingTool = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6) && !isUsingTool)
        {
            useBarrier = true;
            isUsingTool = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7) && !isUsingTool && animator.GetBool("injection1Ready"))
        {
            useInjection1 = true;
            isUsingTool = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8) && !isUsingTool && animator.GetBool("injection2Ready"))
        {
            useInjection2 = true;
            isUsingTool = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9) && !isUsingTool && animator.GetBool("injection3Ready"))
        {
            useInjection3 = true;
            isUsingTool = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0) && !isUsingTool && animator.GetBool("injection4Ready"))
        {
            useInjection4 = true;
            isUsingTool = true;
        }


        // handle tool application
        HandleToolApplication(useTorch, useCutter, useDrone, useMagnet, useSpider, useBarrier, useInjection1, useInjection2, useInjection3, useInjection4, isUsingTool);

        // handle aiming
        HandleAiming(isAiming, aimHorizontal, aimVertical);

        // set animator parameters
        animator.SetBool(useTorchHash, useTorch);
        animator.SetBool(useCutterHash, useCutter);
        animator.SetBool(useSpiderHash, useSpider);
        animator.SetBool(useDroneHash, useDrone);
        animator.SetBool(useMagnetHash, useMagnet);
        animator.SetBool(useBarrierHash, useBarrier);
        animator.SetBool(useInjection1Hash, useInjection1);
        animator.SetBool(useInjection2Hash, useInjection2);
        animator.SetBool(useInjection3Hash, useInjection3);
        animator.SetBool(useInjection4Hash, useInjection4);
        animator.SetBool(isAimingHash, isAiming);
        animator.SetFloat(aimHorizontalHash, aimHorizontal);
        animator.SetFloat(aimVerticalHash, aimVertical);
    }
}
