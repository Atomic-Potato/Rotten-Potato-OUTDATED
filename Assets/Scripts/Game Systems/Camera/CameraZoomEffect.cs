using UnityEngine;

public class CameraZoomEffect : MonoBehaviour, ICameraStrategy
{
    [SerializeField] float zoomInSize;
    [Range(0f, 100f)]
    [SerializeField] float zoomInTime = 0.25f;
    [Range(0f, 100f)]
    [SerializeField] float zoomOutTime = 0.25f;
    
    bool _isZooming;
    public bool IsZooming => _isZooming;

    Transform target;

    float _originalSize;
    float _zoomTimer;

    void Awake()
    {
        _originalSize = Camera.main.orthographicSize;
    }

    void Start()
    {
        target = Player.Instance.transform;
    }

    void ICameraStrategy.ExecuteUpdate()
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
            ZoomUsingUnscaledTime(_originalSize, zoomOutTime);

            if (!_isZooming)
            {
                Camera.main.orthographicSize = _originalSize;
            }
        }  
    }

    void ICameraStrategy.ExecuteFixedUpdate()
    {

    }

    public void ZoomUsingUnscaledTime(float size, float time)
    {
        if (!_isZooming)
        {
            _isZooming = true;
            _zoomTimer = 0f;
        }
        
        if (IsValueReached(size))
        {
            _isZooming = false;
            return;
        }

        _zoomTimer += Time.unscaledDeltaTime;

        float t = Mathf.Clamp01(_zoomTimer / time);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, size, t);

        if (size < Camera.main.orthographicSize)
        {
            transform.position = new Vector3(
                Mathf.Lerp(transform.position.x, target.position.x, t),
                Mathf.Lerp(transform.position.y, target.position.y, t),
                transform.position.z
            );
        }
    }

    public void ZoomUsingScaledTime(float size, float time)
    {
        if (!_isZooming)
        {
            _isZooming = true;
            _zoomTimer = 0f;
        }
        
        if (IsValueReached(size))
        {
            _isZooming = false;
            return;
        }

        _zoomTimer += Time.deltaTime;

        float t = Mathf.Clamp01(_zoomTimer / time);
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, size, t);

        if (size < Camera.main.orthographicSize)
        {
            transform.position = new Vector3(
                Mathf.Lerp(transform.position.x, target.position.x, t),
                Mathf.Lerp(transform.position.y, target.position.y, t),
                transform.position.z
            );
        }
    }

    bool IsValueReached(float size)
    {
        return Mathf.Approximately(Camera.main.orthographicSize, size);
    }
}
