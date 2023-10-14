using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TimeTrialResetPoint : MonoBehaviour
{
    [SerializeField] TimeTrialCourse timeTrialCourse; 
    [SerializeField] bool isShouldRespawn = true;

    void OnTriggerEnter2D(Collider2D other) 
    {
        string tag = other.gameObject.tag;
        if (tag == TagsManager.Tag_Player)
        {
            timeTrialCourse.ResetTrial();
            if (isShouldRespawn)
                pPlayer.Instance.Respawn();
        }    
    }
}
