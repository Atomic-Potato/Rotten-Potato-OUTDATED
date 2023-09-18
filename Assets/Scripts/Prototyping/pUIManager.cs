using TMPro;
using UnityEngine;

public class pUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text hitPoints;
    [SerializeField] TMP_Text dashes;

    string _hitPointsText;
    string _dashesText;

    void Awake() 
    {
        _hitPointsText = hitPoints.text;  
        _dashesText = dashes.text;  
    }

    void OnEnable()
    {
        pPlayer.Instance.Damage += UpdateHitPoints;
        pDash.Instance.AlterDashCount += UpdateDashes;
        UpdateHitPoints();
        UpdateDashes();
    }

    void OnDisable() 
    {
        pPlayer.Instance.Damage -= UpdateHitPoints;
        pDash.Instance.AlterDashCount -= UpdateDashes;
    }

    void UpdateHitPoints(int x = 0)
    {
        hitPoints.text = _hitPointsText + pPlayer.Instance.HitPoints.ToString();
    }

    void UpdateDashes(int x = 0)
    {
        dashes.text = _dashesText + pDash.DashesLeft.ToString();
    }
}
