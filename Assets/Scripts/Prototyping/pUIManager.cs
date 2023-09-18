using TMPro;
using UnityEngine;

public class pUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text hitPoints;
    [SerializeField] TMP_Text dashes;
    [SerializeField] TMP_Text playerState;

    string _hitPointsText;
    string _dashesText;
    string _playerStateText;

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

    void UpdateHitPoints(int x = 0)
    {
        hitPoints.text = _hitPointsText + pPlayer.Instance.HitPoints.ToString();
    }

    void UpdateDashes(int x = 0)
    {
        dashes.text = _dashesText + pDash.DashesLeft.ToString();
    }

    void UpdatePlayerState(AnimationClip x = null)
    {
        if (PlayerAnimationManager.Instance.CurrentClip == null)
        {
            return;
        }
        playerState.text = _playerStateText + PlayerAnimationManager.Instance.CurrentClip.name;
    }
}
