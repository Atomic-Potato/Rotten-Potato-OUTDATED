using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LevelAreaProgressManager : MonoBehaviour 
{
    [SerializeField] bool isShouldResetProgressOnRespawn;
    [SerializeField] List<ProgressArea> progressAreas;

    void Awake() 
    {
        if (isShouldResetProgressOnRespawn)
        {
            pPlayer.Instance.Respawn += ResetAreaProgress;
        }
    
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == Tags.Tag_Player)
        {
            ResetAreaProgress();
        }    
    }

    public void ResetAreaProgress()
    {
        foreach(ProgressArea area in progressAreas)
        {
            area.Reset();
        }
    }
}