using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Gateway : MonoBehaviour
{
    [SerializeField] SpiderInteractive spiderInteractive;
    [SerializeField] float speed;

    // Update is called once per frame
    void Update()
    {
        if (spiderInteractive.InteractionSuccess)
        {
            if (transform.localPosition.y < 2.999)
            {
                transform.localPosition = Vector3.Slerp(transform.localPosition, new Vector3(transform.localPosition.x, 3, transform.localPosition.z), speed * Time.deltaTime);
            }
            else
            {
                transform.localPosition = new Vector3(transform.localPosition.x, 3, transform.localPosition.z);
            }
        }
    }
}
