using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(Collider2D), typeof(Collider2D))]
public class TimeTrialCourse : MonoBehaviour
{
    [SerializeField, Tooltip("Values less than zero mean infinity")] 
    float timeLimit = -1f;
    
    [Space]
    [SerializeField] bool isShouldOffsetTimerOnFinish = true;
    [SerializeField, UnityEngine.Min(0)] 
    float timerOffsetSpeed = 1f;
    [SerializeField] Vector2 timerOffset;

    [Space]
    [SerializeField] Collider2D startTrigger;
    [SerializeField] Collider2D finishTrigger;

    Timer _timer;
    Color _originalTimerColor;

    void Awake()
    {
        _timer = new Timer(false);
        startTrigger.enabled = true;
        finishTrigger.enabled = false;
    }

    void Start()
    {
        _originalTimerColor = BackgroundUIManager.Instance.TimerColor;
    }

    void Update()
    {
        if (_timer.IsStarted && !_timer.IsPaused)
        {
            _timer.Count();
            BackgroundUIManager.Instance.UpdateTimer(_timer.CurrentTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        string tag = other.gameObject.tag;
        if (tag == TagsManager.Tag_Player)
        {
            if (!_timer.IsStarted)
                StartTrial();
            else
                FinishTrial();
        }    
    }

    #region Public Methods
    public void StartTrial()
    {
        ResetTrial();
        _timer.Start();
        BackgroundUIManager.Instance.ShowTimer();
        startTrigger.enabled = false;
        finishTrigger.enabled = true;
    }

    public void FinishTrial()
    {
        Color brightTimerColor = _originalTimerColor;
        brightTimerColor.a = 1f;

        _timer.Stop();
        BackgroundUIManager.Instance.TimerPosition = timerOffset;
        BackgroundUIManager.Instance.TimerColor = brightTimerColor; 
        finishTrigger.enabled = false;
    }

    public void ResetTrial()
    {
        _timer.Reset();
        BackgroundUIManager.Instance.TimerPosition = Vector3.zero;
        BackgroundUIManager.Instance.TimerColor = _originalTimerColor; 
        startTrigger.enabled = true;
        finishTrigger.enabled = false;
    }
    #endregion
}