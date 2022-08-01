using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedrunTimer : MonoBehaviour
{
    [SerializeField] Text timer;
    
    [HideInInspector] public bool finished;
    [HideInInspector] public float currentTime = 0;

    void Awake() 
    {
        currentTime = 0;
        finished = false;
    }
   
    // Update is called once per frame
    void Update()
    {
        if(!finished)
        {
            currentTime += Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            timer.text = time.Minutes.ToString("0") + ":" + time.Seconds.ToString("00") + ":" + ((int)time.Milliseconds/100).ToString(); 
        }
    }
}
