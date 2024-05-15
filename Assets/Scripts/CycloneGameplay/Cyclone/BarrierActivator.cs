using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BarrierActivator : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(ManageActivation());
        StartCoroutine(ManageLifetime());
    }

    IEnumerator ManageActivation()
    {
        float timePassed = 0;

        while (timePassed < 1)
        {
            timePassed += Time.deltaTime;

            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(0.5f, 0.5f, 0.5f), timePassed);

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator ManageLifetime()
    {
        float timePassed = 0;

        while (timePassed < 10)
        {
            timePassed += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        StartCoroutine(ManageDeactivation());
    }

    IEnumerator ManageDeactivation()
    {
        float timePassed = 0;

        while (timePassed < 1)
        {
            timePassed += Time.deltaTime;

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, timePassed);

            yield return new WaitForEndOfFrame();
        }

        gameObject.SetActive(false);
    }
}
