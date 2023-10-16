using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TimeTrialResetPoint : MonoBehaviour
{
    [SerializeField] TimeTrialCourse timeTrialCourse;
    [SerializeField] bool isShouldRespawn = true;
    public void ResetTrial()
    {
        timeTrialCourse.ResetTrial();
        if (isShouldRespawn)
            pPlayer.Instance.Respawn();
    }
}
