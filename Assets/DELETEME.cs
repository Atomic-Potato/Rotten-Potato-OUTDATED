using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DELETEME : MonoBehaviour
{
    public float target;
    public float time;
    public float acceleration;
    public bool start;

    public float current = 0;

    void Start()
    {
        acceleration = target/time;
    }

    void Update()
    {
        if(current < target && start)
            current += acceleration * Time.deltaTime;

        Debug.Log("Current: " + current);
    }
}
