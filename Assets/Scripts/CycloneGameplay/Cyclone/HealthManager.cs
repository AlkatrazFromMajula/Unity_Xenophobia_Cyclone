using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    float recentHealth;

    // Start is called before the first frame update
    void Start()
    {
        recentHealth = 100;
    }

    public void ReduceHealth(float value)
    {
        recentHealth -= value;
        print("YES");
    }

    // Update is called once per frame
    void Update()
    {
        if (recentHealth > 0)
        {
            print(recentHealth);
        }
        else
        {
            print("DEAD");
        }
    }
}
