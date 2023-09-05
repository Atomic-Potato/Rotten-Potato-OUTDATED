using UnityEngine;

public class pLevelManager : MonoBehaviour
{
    [SerializeField]
    Transform respawnPoint;

    public static Transform RespawnPoint;

    void Awake() 
    {
        RespawnPoint = respawnPoint;    
    }
}
