using TMPro;
using UnityEngine;

public class MediumEnemyUIManager : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] string idleDescription;
    [SerializeField] Color idleColor = new Color(1, 0, 0);

    [Space]
    [SerializeField] string counterAttackDescription;
    [SerializeField] Color counterAttackColor = new Color(1, 1, 0);

    [Space]
    [SerializeField] string attackDescription;
    [SerializeField] Color attackColor = new Color(1, 0, 1);

    [Space]
    [SerializeField] TMP_Text stateText;
    [SerializeField] TMP_Text hitPointsText;

    [Space]
    [SerializeField] MediumEnemy enemy;
    [SerializeField] Canvas canvas;
    #endregion

    #region Global Variables
    string hitPointsDescription;
    #endregion

    void Awake() 
    {
        canvas.worldCamera = Camera.main;
        hitPointsDescription = hitPointsText.text;
        UpdateHitPoints();
    }

    void OnEnable()
    {
        enemy.UpdateHitPoints += UpdateHitPoints;       
    }

    void OnDisable()
    {
        enemy.UpdateHitPoints -= UpdateHitPoints;       
    }

    void Update()
    {
        // Enemy State
        if (enemy.IsAttacking)
            DisplayAttackState();
        else if (enemy.IsCounterAttacking)
            DisplayCounterAttackState();
        else
            DisplayIdleState();
    }

    #region States
    void DisplayAttackState()
    {
        stateText.color = attackColor;
        stateText.text = attackDescription + enemy.ToBeParriedTimer.ToString();
    }

    void DisplayCounterAttackState()
    {
        stateText.color = counterAttackColor;
        stateText.text = counterAttackDescription + enemy.CounterAttackTimer.ToString();
    }

    void DisplayIdleState()
    {
        if (stateText.color == idleColor)
            return;
        stateText.color = idleColor;
        stateText.text = idleDescription;
    }
    #endregion

    void UpdateHitPoints()
    {
        hitPointsText.text = hitPointsDescription + enemy.HitPoints.ToString();
    }
}
