using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVTorchFlashlightActivator : FlashlightActivator
{

    [SerializeField] GameObject revealingBeam;

    override protected void OnEnable()
    {
        base.OnEnable();
        revealingBeam.SetActive(false);
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();

        revealingBeam.SetActive(active);
    }
}
