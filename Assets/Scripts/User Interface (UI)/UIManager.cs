using System;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
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
        pPlayer.Instance.UpdateHitPoints += UpdateHitPoints;
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
        pPlayer.Instance.UpdateHitPoints -= UpdateHitPoints;
        pDash.Instance.AlterDashCount -= UpdateDashes;
        PlayerAnimationManager.Instance.AnimationStateAction -= UpdatePlayerState;
    }

    #endregion

    #region UI Updates
    void UpdateHitPoints()
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

    #endregion
}
