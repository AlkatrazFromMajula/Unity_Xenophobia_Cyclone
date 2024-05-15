using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class MagnetActivator : MonoBehaviour
{
    MagnetInteractive[] magnets;
    bool magnetizing;
    bool magnetizingAdd;

    public bool IsMagnetizing { set { magnetizing = value; } }

    void Magnetize()
    {
        magnets = FindObjectsOfType<MagnetInteractive>();
        foreach (MagnetInteractive magnet in magnets)
        {
            GameObject magnetObj = magnet.gameObject;
            float distance = (transform.position - magnetObj.transform.position).magnitude;

            if (distance <= 50)
            {
                if (magnet.gameObject.CompareTag("Metal Trash"))
                {
                    StartCoroutine(PullTrash(magnet, distance));
                }

                magnet.InteractionFinished = true;
            }
        }
    }

    IEnumerator PullTrash(MagnetInteractive magnet, float distance)
    {
        float timePassed = 0f;


        while (timePassed < 2f)
        {
            float strength = 1 - distance / 50;

            GameObject magnetObj = magnet.gameObject;
            Vector3 direction = (transform.position - magnetObj.transform.position).normalized;
            magnetObj.GetComponent<Rigidbody>().AddForce(direction * 50 * magnetObj.GetComponent<Rigidbody>().mass * strength, ForceMode.Force);

            timePassed += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        magnetizingAdd = false;
        magnetizing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (magnetizing && !magnetizingAdd)
        {
            Magnetize();
            magnetizingAdd = true;
        }
    }
}
