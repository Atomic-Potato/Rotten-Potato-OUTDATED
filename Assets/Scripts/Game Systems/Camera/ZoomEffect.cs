using System;
using UnityEngine;

public class ZoomEffect : MonoBehaviour
{
    [SerializeField] float zoomInSize;
    [Range(0f, 100f)]
    [SerializeField] float zoomInTime = 0.25f;
    [Range(0f, 100f)]
    [SerializeField] float zoomOutTime = 0.25f;
    
    float _originalSize;
    float _zoomTimer;
    bool _isZoomingIn;

    void Awake()
    {
        _originalSize = Camera.main.orthographicSize;
    }

    void Update()
    {
        if (Time.timeScale < 1f)
        {
            if (!IsValueReached(zoomInSize))
            {
                ZoomUsingUnscaledTime(zoomInSize, zoomInTime);
            }
        }
        else
        {
            if (!IsValueReached(_originalSize))
            {
                ZoomUsingUnscaledTime(_originalSize, zoomOutTime);
            }
            else if (Camera.main.orthographicSize != _originalSize)
            {
                Camera.main.orthographicSize = _originalSize;
            }
        }  
    }

    public void ZoomUsingUnscaledTime(float size, float time)
    {
        if (!_isZoomingIn)
        {
            _isZoomingIn = true;
            _zoomTimer = 0f;
        }
        
        if (IsValueReached(size))
        {
            _isZoomingIn = false;
            return;
        }

        _zoomTimer += Time.unscaledDeltaTime;

        float t = Mathf.Clamp01(_zoomTimer / time);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, size, t);
    }

    public void ZoomUsingScaledTime(float size, float time)
    {
        if (!_isZoomingIn)
        {
            _isZoomingIn = true;
            _zoomTimer = 0f;
        }
        
        if (IsValueReached(size))
        {
            _isZoomingIn = false;
            return;
        }

        _zoomTimer += Time.deltaTime;

        float t = Mathf.Clamp01(_zoomTimer / time);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, size, t);
    }

    bool IsValueReached(float size)
    {
        return Mathf.Approximately(Camera.main.orthographicSize, size);
    }
}
