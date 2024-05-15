using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    [SerializeField] float injectionReactivationDelayTime;
    Animator wireAnimator;
    MagnetActivator magnetActivator;
    Timer injection1DelayTimer;
    Timer injection2DelayTimer;
    Timer injection3DelayTimer;
    Timer injection4DelayTimer;
    [SerializeField] GameObject wire;
    [SerializeField] GameObject barrier;
    [SerializeField] GameObject wireUVTorch;
    [SerializeField] GameObject wireCutter;
    [SerializeField] GameObject wireSpider;
    [SerializeField] GameObject wireDrone;
    [SerializeField] GameObject wireHealthShot;
    [SerializeField] GameObject wireOcularShot;
    [SerializeField] GameObject wireNozzle;
    [SerializeField] GameObject nozzle;
    [SerializeField] Transform wireLastBone;

    [SerializeField] GameObject vestUVTorch;
    [SerializeField] GameObject vestCutter;
    [SerializeField] GameObject vestSpider;
    [SerializeField] GameObject vestDrone;
    [SerializeField] GameObject vestMagnet;
    [SerializeField] GameObject vestBarrier;
    [SerializeField] GameObject vestInjection1;
    [SerializeField] GameObject vestInjection2;
    [SerializeField] GameObject vestInjection3;
    [SerializeField] GameObject vestInjection4;

    GameObject[] wireTools = new GameObject[6];
    GameObject[] vestTools = new GameObject[10];

    bool injection1Ready = true;
    bool injection2Ready = true;
    bool injection3Ready = true;
    bool injection4Ready = true;

    // increase performance 
    int injection1ReadyHash;
    int injection2ReadyHash;
    int injection3ReadyHash;
    int injection4ReadyHash;

    // Start is called before the first frame update
    void Start()
    {
        wireAnimator = wire.GetComponent<Animator>();
        magnetActivator = GetComponentInChildren<MagnetActivator>();

        injection1ReadyHash = Animator.StringToHash("injection1Ready");
        injection2ReadyHash = Animator.StringToHash("injection2Ready");
        injection3ReadyHash = Animator.StringToHash("injection3Ready");
        injection4ReadyHash = Animator.StringToHash("injection4Ready");

        wireTools[0] = wireUVTorch;
        wireTools[1] = wireCutter;
        wireTools[2] = wireSpider;
        wireTools[3] = wireDrone;
        wireTools[4] = wireHealthShot;
        wireTools[5] = wireOcularShot;

        vestTools[0] = vestUVTorch;
        vestTools[1] = vestCutter;
        vestTools[2] = vestSpider;
        vestTools[3] = vestDrone;
        vestTools[4] = vestMagnet;
        vestTools[5] = vestBarrier;
        vestTools[6] = vestInjection1;
        vestTools[7] = vestInjection2;
        vestTools[8] = vestInjection3;
        vestTools[9] = vestInjection4;
    }

    void HandleAimToolActivation()
    {
        // handle activation
        if (wireAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wire_GetTorch_reversed"))
        {
            nozzle.SetActive(true);
            wireNozzle.SetActive(false);
            wireTools[0].SetActive(true);
            vestTools[0].SetActive(false);
        }
        else if (wireAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wire_GetCutter_reversed"))
        {
            nozzle.SetActive(true);
            wireNozzle.SetActive(false);
            wireTools[1].SetActive(true);
            vestTools[1].SetActive(false);
        }
        else if (wireAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wire_GetSpider_reversed"))
        {
            wireTools[2].SetActive(true);
            vestTools[2].SetActive(false);

            wireTools[2].GetComponent<SpiderController>().enabled = false;
            wireTools[2].GetComponent<SpiderActivationManager>().enabled = false;
            wireTools[2].transform.SetParent(wireLastBone);
            wireTools[2].transform.localPosition = new Vector3(0, 0.0003f, 0);
            wireTools[2].transform.up = wireLastBone.up * -1;

        }
        else if (wireAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wire_GetDrone_reversed"))
        {
            wireTools[3].SetActive(true);
            vestTools[3].SetActive(false);

            wireTools[3].GetComponent<DronePhysiksManager>().enabled = false;
            wireTools[3].GetComponent<DroneActivationManager>().enabled = false;

            wireTools[3].transform.SetParent(wireLastBone);
            wireTools[3].transform.localPosition = Vector3.zero;
            wireTools[3].transform.up = wireLastBone.up;
        }
        else if (wireAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wire_GetMagnet_reversed"))
        {
            magnetActivator.IsMagnetizing = true;
        }
        else if (wireAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wire_GetBarrier_reversed"))
        {
            barrier.SetActive(true);
        }

        // handle deactivation
        if (wireAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wire_Prepare_reversed"))
        {
            nozzle.SetActive(false);
            wireNozzle.SetActive(true);
            wireTools[0].SetActive(false);
            wireTools[1].SetActive(false);
            vestTools[0].SetActive(true);
            vestTools[1].SetActive(true);
        }

        if (wireAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wire_UseSpider_reversed"))
        {
            wireTools[2].transform.SetParent(null);
            wireTools[2].GetComponent<SpiderActivationManager>().enabled = true;
        }

        if (wireAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wire_UseDrone_reversed"))
        {
            wireTools[3].transform.SetParent(null);
            wireTools[3].GetComponent<DroneActivationManager>().enabled = true;
            wireTools[3].GetComponent<DroneAnimationManager>().Activate = true;
        }

    }

    void HandleSpiderAndDroneDeactivation()
    {
        if (wireTools[2].activeSelf || wireTools[3].activeSelf)
        {
            GetComponent<CycloneAnimationManager>().UsingCharacterTool = true;
            vestTools[3].SetActive(!wireTools[3].activeSelf);
            vestTools[2].SetActive(!wireTools[2].activeSelf);
        }
        else
        {
            GetComponent<CycloneAnimationManager>().UsingCharacterTool = false;
            vestTools[3].SetActive(true);
            vestTools[2].SetActive(true);
        }
    }

    void HandleInjectionActivation()
    {
        // handle activation
        if (wireAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wire_GetInjection1_reversed") && injection1Ready)
        {
            wireHealthShot.SetActive(true);
            vestTools[6].SetActive(false);
            injection1Ready = false;
            injection1DelayTimer = gameObject.AddComponent<Timer>();
            injection1DelayTimer.Duration = injectionReactivationDelayTime;
            injection1DelayTimer.Run();
        }
        else if (wireAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wire_GetInjection2_reversed") && injection2Ready)
        {
            wireHealthShot.SetActive(true);
            vestTools[7].SetActive(false);
            injection2Ready = false;
            injection2DelayTimer = gameObject.AddComponent<Timer>();
            injection2DelayTimer.Duration = injectionReactivationDelayTime;
            injection2DelayTimer.Run();
        }
        else if (wireAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wire_GetInjection3_reversed") && injection3Ready)
        {
            wireOcularShot.SetActive(true);
            vestTools[8].SetActive(false);
            injection3Ready = false;
            injection3DelayTimer = gameObject.AddComponent<Timer>();
            injection3DelayTimer.Duration = injectionReactivationDelayTime;
            injection3DelayTimer.Run();
        }
        else if (wireAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wire_GetInjection4_reversed") && injection4Ready)
        {
            wireOcularShot.SetActive(true);
            vestTools[9].SetActive(false);
            injection4Ready = false;
            injection4DelayTimer = gameObject.AddComponent<Timer>();
            injection4DelayTimer.Duration = injectionReactivationDelayTime;
            injection4DelayTimer.Run();
        }

        // handle deactivation
        if (wireAnimator.GetCurrentAnimatorStateInfo(0).IsName("Wire_Prepare_reversed"))
        {
            wireHealthShot.SetActive(false);
            wireOcularShot.SetActive(false);
        }

        if (!injection1Ready)
        {
            if (injection1DelayTimer.Finished)
            {
                injection1Ready = true;
                vestTools[6].SetActive(true);
                Destroy(injection1DelayTimer);
            }
        }
        if (!injection2Ready)
        {
            if (injection2DelayTimer.Finished)
            {
                injection2Ready = true;
                vestTools[7].SetActive(true);
                Destroy(injection2DelayTimer);
            }
        }
        if (!injection3Ready)
        {
            if (injection3DelayTimer.Finished)
            {
                injection3Ready = true;
                vestTools[8].SetActive(true);
                Destroy(injection3DelayTimer);
            }
        }
        if (!injection4Ready)
        {
            if (injection4DelayTimer.Finished)
            {
                injection4Ready = true;
                vestTools[9].SetActive(true);
                Destroy(injection4DelayTimer);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // handle aim tools (torch & cutter)
        HandleAimToolActivation();

        // handle injection application
        HandleInjectionActivation();

        HandleSpiderAndDroneDeactivation();

        // set animator values
        wireAnimator.SetBool(injection1ReadyHash, injection1Ready);
        wireAnimator.SetBool(injection2ReadyHash, injection2Ready);
        wireAnimator.SetBool(injection3ReadyHash, injection3Ready);
        wireAnimator.SetBool(injection4ReadyHash, injection4Ready);
    }
}
