using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    public static SpawnManager Instance;
    [HideInInspector] public Vector2? LevelSpawnPoint;
    [HideInInspector] public Vector2? Checkpoint;

    void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }


    public void SpawnPlayer()
    {
        if (pPlayer.Instance != null)
            throw new System.Exception("Failed to spawn player: Player already spawned");

        if (LevelSpawnPoint == null)
            throw new System.Exception("Failed to spawn player: No spawn point set");

        Instantiate(playerPrefab, (Vector2)LevelSpawnPoint, Quaternion.identity);
    }

    public void SpawnPlayerAtCheckpoint()
    {
        if (pPlayer.Instance != null)
            throw new System.Exception("Failed to spawn player: Player already spawned");

        if (Checkpoint == null)
            throw new System.Exception("Failed to spawn player: No checkpoint set");

        Instantiate(playerPrefab, (Vector2)Checkpoint, Quaternion.identity);
    }
}
