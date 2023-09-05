using TMPro;
using UnityEngine;

public class pUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text hitPoints;

    string _hitPointsText;

    void Awake() 
    {
        _hitPointsText = hitPoints.text;    
    }

    void OnEnable()
    {
        pPlayer.Instance.Damage += (x) => UpdateHitPoints();
        UpdateHitPoints();
    }

    void OnDisable() 
    {
        pPlayer.Instance.Damage -= (x) => UpdateHitPoints();
    }

    void UpdateHitPoints()
    {
        hitPoints.text = _hitPointsText + pPlayer.Instance.HitPoints.ToString();
    }
}
