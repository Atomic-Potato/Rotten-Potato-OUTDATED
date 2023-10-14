using System;
using UnityEngine;
using TMPro;

public class pUIManager : MonoBehaviour
{
    #region Inspector
    [SerializeField] TMP_Text hitPoints;
    [SerializeField] TMP_Text dashes;
    [SerializeField] TMP_Text playerState;
    [SerializeField] TMP_Text screenTimer;

    [Space]
    [Header("Menus")]
    [SerializeField] GameObject menuPause;
    #endregion

    #region Global Variables
    string _hitPointsText;
    string _dashesText;
    string _playerStateText;
    float _currentTimeScale;
    bool _isPausedGame;

    static TMP_Text timer;
    #endregion

    #region Execution
    void Awake() 
    {
        _hitPointsText = hitPoints.text;  
        _dashesText = dashes.text;  
        _playerStateText = playerState.text;
        timer = screenTimer;
    }

    void OnEnable()
    {
        pPlayer.Instance.Damage += UpdateHitPoints;
        pDash.Instance.AlterDashCount += UpdateDashes;
        PlayerAnimationManager.Instance.AnimationStateAction += UpdatePlayerState;
        UpdateHitPoints();
        UpdateDashes();
        UpdatePlayerState();
    }

    void OnDisable() 
    {
        pPlayer.Instance.Damage -= UpdateHitPoints;
        pDash.Instance.AlterDashCount -= UpdateDashes;
        PlayerAnimationManager.Instance.AnimationStateAction -= UpdatePlayerState;
    }

    void Update()
    {
        if (PlayerInputManager.Maps.UI.Pause.triggered)
        {
            if (_isPausedGame == false)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
    }
    #endregion

    #region UI Updates
    void UpdateHitPoints(int x = 0)
    {
        hitPoints.text = _hitPointsText + pPlayer.Instance.HitPoints.ToString();
    }

    void UpdateDashes(int x = 0)
    {
        dashes.text = _dashesText + pDash.DashesLeft.ToString();
    }

    void UpdatePlayerState(AnimationClip x = null, bool y = false, float z = 0f)
    {
        if (PlayerAnimationManager.Instance.CurrentClip == null)
        {
            return;
        }
        playerState.text = _playerStateText + PlayerAnimationManager.Instance.CurrentClip.name;
    }

    public static void ShowTimer()
    {
        if (timer != null && !timer.enabled)
            timer.enabled = true;
        else if (timer == null)
            throw new Exception("No UI timer text is set");
    }

    public static void UpdateTimer(float time)
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

    public static void HideTimer()
    {
        if (timer != null && timer.enabled)
            timer.enabled = false;
        else if (timer == null)
            throw new Exception("No UI timer text is set");
    }
    #endregion

    #region Methods
    public void ResumeGame()
    {
        _isPausedGame = false;
        Time.timeScale = _currentTimeScale;
        menuPause.SetActive(false);
    }

    public void PauseGame()
    {
        _isPausedGame = true;
        _currentTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        menuPause.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}
