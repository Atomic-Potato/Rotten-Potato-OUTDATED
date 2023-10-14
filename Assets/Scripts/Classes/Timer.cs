using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Timer : MonoBehaviour
{
    bool isStarted;
    public bool IsStarted => isStarted;
    bool isFinished;
    public bool IsFinished => isFinished;
    public bool isPaused;
    public bool IsPaused => isPaused;
    float currentTime;
    public float CurrentTime => currentTime;

    bool _isUsingUnscaledTime;
    public bool IsUsingUnscaledTime => _isUsingUnscaledTime;

    private Timer(){}
    
    public Timer(bool isUsingUnscaledTime)
    {
        _isUsingUnscaledTime = isUsingUnscaledTime;
    }
   
    void Awake() 
    {
        currentTime = 0f;
    }
   
    public void Start()
    {
        Reset();
        isStarted = true;
    }

    public void Stop()
    {
        if (isStarted == true)
            isStarted = false;
        if (isFinished == false)
            isFinished = true;
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }

    public void Reset()
    {
        if (isStarted != false)
            isStarted = false;
        if (isFinished != false)
            isFinished = false;
        if (isPaused == true)
            isPaused = false;
        currentTime = 0f;
    }

    public void Count()
    {
        if (!isStarted || isPaused)
            return;
            
        currentTime = _isUsingUnscaledTime ? 
            currentTime + Time.unscaledDeltaTime : 
            currentTime + Time.deltaTime; 
    }
}
