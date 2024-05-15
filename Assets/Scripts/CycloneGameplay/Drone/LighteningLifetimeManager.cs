using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public class LighteningLifetimeManager : MonoBehaviour
{
    [SerializeField] Material material;
    [SerializeField] float timestamp;
    [SerializeField] ParticleSystem[] particles;
    [SerializeField] Transform placeholder;
    [SerializeField] GameObject preLightening;
    [SerializeField] Transform aimTarget;
    [SerializeField] float stratch;
    bool particlesPlaying;
    float timePassed;
    float visibility;

    void OnEnable()
    {
        foreach (ParticleSystem particle in particles)
        {
            particle.Clear();
            particle.Stop();
        }

        particlesPlaying = false;
        timePassed = 0;
        visibility = 0;
        material.SetFloat("_Clip", 0f);

    }

    IEnumerator HandleVisibility()
    {

        while (visibility < 1f)
        {
            visibility += Time.deltaTime * 0.5f;

            yield return new WaitForEndOfFrame();
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (timePassed >= timestamp)
        {
            foreach (ParticleSystem particle in particles)
            {
                if (particle.isPlaying)
                {
                    particlesPlaying = true;
                }
            }

            if (!particlesPlaying)
            {
                float distance = (aimTarget.position - placeholder.position).magnitude;

                foreach (ParticleSystem particle in particles)
                {
                    particle.transform.localScale = new Vector3(particle.transform.localScale.x, distance * stratch, particle.transform.localScale.z);
                    particle.Play();
                }
                StartCoroutine(HandleVisibility());

                if (!GetComponentInParent<DronePhysiksManager>().GetLighteningStrike)
                {
                    GetComponentInParent<DronePhysiksManager>().LighteningStrike = true;
                }

                if (!GetComponentInParent<DroneAnimationManager>().GetHit)
                {
                    GetComponentInParent<DroneAnimationManager>().Hit = true;
                }

                gameObject.GetComponent<LighteningRaycast>().Shoot = true;

                transform.forward = (aimTarget.position - placeholder.position).normalized;
                transform.SetParent(null);


            }

            preLightening.SetActive(false);
        }

        if (visibility <= 1)
            material.SetFloat("_Clip", Mathf.Clamp(visibility, 0, 1));
        else
        {
            gameObject.SetActive(false);
            transform.SetParent(placeholder);
            transform.localEulerAngles = Vector3.zero;
            transform.localPosition = Vector3.zero;
        }

        timePassed += Time.deltaTime;

    }
}
