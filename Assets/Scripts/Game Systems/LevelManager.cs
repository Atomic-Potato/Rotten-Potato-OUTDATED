using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Transform mainRespawnPoint;

    public static Transform RespawnPoint;

    void Awake() 
    {
        RespawnPoint = mainRespawnPoint;
        Time.timeScale = 1f;
    }
}
