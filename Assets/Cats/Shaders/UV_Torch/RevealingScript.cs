using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RevealingScript : MonoBehaviour
{

    public Material reveal;
    public Light light;

    // Start is called before the first frame update
    void Start()
    {
        reveal.SetVector("_LightPosition", new Vector4(0, 0, 0, 0));
        reveal.SetVector("_LightDirection", new Vector4(0, 0, 0, 0));
    }

    void OnEnable()
    {
        reveal.SetVector("_LightPosition", new Vector4(0,0,0,0));
        reveal.SetVector("_LightDirection", new Vector4(0,0,0,0));
    }

    private void OnDisable()
    {
        reveal.SetVector("_LightPosition", new Vector4(0, 0, 0, 0));
        reveal.SetVector("_LightDirection", new Vector4(0, 0, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        reveal.SetVector("_LightPosition", light.transform.position);
        reveal.SetVector("_LightDirection", -light.transform.forward);
        reveal.SetFloat("_LightAngle", light.spotAngle);
    }
}
