using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshCollider : MonoBehaviour
{
    SkinnedMeshRenderer meshRenderer;
    MeshCollider col;
    Animator anim;
    [SerializeField] float scale;
    [SerializeField] Vector3 newScale;

    private void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        col = GetComponent<MeshCollider>();
        anim = GetComponentInParent<Animator>();
        col.transform.localScale = col.transform.localScale / scale;
        transform.localScale = newScale;
        UpdateCollider();
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            UpdateCollider();
        }
        else
        {
            print("not");
        }
    }

    public void UpdateCollider()
    {
        Mesh colliderMesh = new Mesh();
        meshRenderer.BakeMesh(colliderMesh);
        col.sharedMesh = null;
        col.sharedMesh = colliderMesh;
    }

}
