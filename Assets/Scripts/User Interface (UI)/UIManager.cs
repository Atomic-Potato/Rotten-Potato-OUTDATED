using System;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region Inspector
    [SerializeField] TMP_Text hitPoints;
    [SerializeField] TMP_Text dashes;
    [SerializeField] TMP_Text playerState;
    [SerializeField] TMP_Text timer;

    [Space]
    [Header("Menus")]
    [SerializeField] GameObject menuPause;
    #endregion

    #region Global Variables
    static UIManager _instance;
    public static UIManager Instance => _instance;
    string _hitPointsText;
    string _dashesText;
    string _playerStateText;

    #endregion

    #region Execution
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

        _hitPointsText = hitPoints.text;  
        _dashesText = dashes.text;  
        _playerStateText = playerState.text;
    }

    void OnEnable()
    {
        pPlayer.Instance.Damage += UpdateHitPoints;
        pDash.Instance.AlterDashCount += UpdateDashes;
        PlayerAnimationManager.Instance.AnimationStateAction += UpdatePlayerState;
        GameManager.Instance.PauseGame += PauseGame;
        GameManager.Instance.ResumeGame += ResumeGame;

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
    public void ResumeGame()
    {
        GameManager.Instance.PauseGame();
        menuPause.SetActive(false);
    }

    public void PauseGame()
    {
        GameManager.Instance.ResumeGame();
        menuPause.SetActive(true);
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

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
}
