using TMPro;
using UnityEngine;

public class pUIManager : MonoBehaviour
{
    #region Inspector
    [SerializeField] TMP_Text hitPoints;
    [SerializeField] TMP_Text dashes;
    [SerializeField] TMP_Text playerState;

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
    #endregion

    #region Execution
    void Awake() 
    {
        _hitPointsText = hitPoints.text;  
        _dashesText = dashes.text;  
        _playerStateText = playerState.text;
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
