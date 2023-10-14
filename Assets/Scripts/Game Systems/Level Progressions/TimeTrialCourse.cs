using UnityEngine;
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
        timer = new Timer();
        startTrigger.enabled = true;
        finishTrigger.enabled = false;
    }

    
}