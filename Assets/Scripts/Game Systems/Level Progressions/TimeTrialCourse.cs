using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Collider2D))]
public class TimeTrialCourse : MonoBehaviour
{
    [SerializeField, Tooltip("Values less than zero mean infinity")] 
    float timeLimit = -1f;
    
    [Space]
    [SerializeField] Collider2D startTrigger;
    [SerializeField] Collider2D finishTrigger;

    Timer timer;

    void Awake()
    {
        timer = new Timer(false);
        startTrigger.enabled = true;
        finishTrigger.enabled = false;
    }

    void Update()
    {
        if (timer.IsStarted && !timer.IsPaused)
        {
            timer.Count();
            BackgroundUIManager.Instance.UpdateTimer(timer.CurrentTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        string tag = other.gameObject.tag;
        if (tag == TagsManager.Tag_Player)
        {
            if (!timer.IsStarted)
                StartTrial();
            else
                FinishTrial();
        }    
    }

    public void StartTrial()
    {
        ResetTrial();
        timer.Start();
        BackgroundUIManager.Instance.ShowTimer();
        startTrigger.enabled = false;
        finishTrigger.enabled = true;
    }

    public void FinishTrial()
    {
        timer.Stop();
        finishTrigger.enabled = false;
    }

    public void ResetTrial()
    {
        timer.Reset();
        startTrigger.enabled = true;
        finishTrigger.enabled = false;
    }

}