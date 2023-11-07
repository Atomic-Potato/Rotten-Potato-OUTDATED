using TMPro;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class BackgroundUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text freeDash;
    
    [Space]
    [SerializeField] TMP_Text timer;
    [SerializeField] RectTransform timerTransfrom;

    [Space]
    [SerializeField] Canvas canvas;

    static BackgroundUIManager _instance;
    public static BackgroundUIManager Instance => _instance;

    public Vector3 TimerPosition
    {
        get { return timerTransfrom.anchoredPosition; }
        set { timerTransfrom.anchoredPosition = value; }
    }

    public Color TimerColor 
    {
        get { return timer.color; }
        set { timer.color = value; }
    }

    Vector3 refTimerOffsetVelocity;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        freeDash.enabled = false;
        timer.enabled = false;
        canvas.worldCamera = Camera.main;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += ResetOnSceneLoad;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= ResetOnSceneLoad;
    }

    void Update()
    {
        if (Parry.IsGivenFreeDash)
        {
            if (!freeDash.enabled)
                freeDash.enabled = true;
        }
        else
        {
            if (freeDash.enabled)
                freeDash.enabled = false;
        }
    }

    #region UI Updates
    public void UpdateTimer(float time)
    {
        if (timer == null)
            throw new Exception("No UI timer text is set");
        if (!timer.enabled)
            return;

        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        timer.text = timeSpan.Minutes.ToString("0") + ":" 
            + timeSpan.Seconds.ToString("00") + ":" 
            + ((int)timeSpan.Milliseconds/100).ToString();
    }
    #endregion

    #region Methods
    public void ShowTimer()
    {
        if (timer != null && !timer.enabled)
            timer.enabled = true;
        else if (timer == null)
            throw new Exception("No UI timer text is set");
    }

    public void HideTimer()
    {
        if (timer != null && timer.enabled)
            timer.enabled = false;
        else if (timer == null)
            throw new Exception("No UI timer text is set");
    }
    #endregion

    void ResetOnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        freeDash.enabled = false;
        timer.enabled = false;
        canvas.worldCamera = Camera.main;
    }
}
