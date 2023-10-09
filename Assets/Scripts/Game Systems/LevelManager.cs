using UnityEngine;

public class LevelManager : MonoBehaviour 
{
    void Awake()
    {
        Time.timeScale = 1f;
        if (SpawnManager.Instance.Checkpoint != null)
            SpawnManager.Instance.SpawnPlayerAtCheckpoint();
        else 
            SpawnManager.Instance.SpawnPlayer();
    }    
}