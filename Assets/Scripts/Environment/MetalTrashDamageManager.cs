using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MetalTrashDamageManager : MonoBehaviour
{
    UnityEvent<float> reduceHealthEvent;
    Rigidbody rb;
    float damage;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        reduceHealthEvent = new UnityEvent<float>();
        reduceHealthEvent.AddListener(FindObjectOfType<HealthManager>().gameObject.GetComponent<HealthManager>().ReduceHealth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Cyclone"))
        {
            if (rb.velocity.magnitude > 25)
                damage = rb.velocity.magnitude * rb.mass * 5;
            else
                damage = 0;

            reduceHealthEvent.Invoke(damage);
        }
    }
}
