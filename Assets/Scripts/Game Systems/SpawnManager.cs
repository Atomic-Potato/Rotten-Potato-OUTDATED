using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Range(0f, 100f)]
    [SerializeField] float timeToSpawn = 1f; 
    [SerializeField] GameObject playerPrefab;

    public static SpawnManager Instance;
    [HideInInspector] public Vector2? LevelSpawnPoint;
    [HideInInspector] public Vector2? Checkpoint;

    GameObject _spawnedPlayerObject;
    Coroutine _spawnCache;

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

        if (_spawnCache == null)
            _spawnCache = StartCoroutine(ExecuteSpawn());

        IEnumerator ExecuteSpawn()
        {
            _spawnedPlayerObject = Instantiate(playerPrefab, (Vector2)LevelSpawnPoint, Quaternion.identity);
            _spawnedPlayerObject.SetActive(false);
            yield return new WaitForSecondsRealtime(timeToSpawn);
            _spawnedPlayerObject.SetActive(true);
            _spawnCache = null;
        }
    }

    public void SpawnPlayerAtCheckpoint()
    {
        if (pPlayer.Instance != null)
            throw new System.Exception("Failed to spawn player: Player already spawned");

        if (Checkpoint == null)
            throw new System.Exception("Failed to spawn player: No checkpoint set");

        if (_spawnCache == null)
            _spawnCache = StartCoroutine(ExecuteSpawn());

        IEnumerator ExecuteSpawn()
        {
            _spawnedPlayerObject = Instantiate(playerPrefab, (Vector2)Checkpoint, Quaternion.identity);
            _spawnedPlayerObject.SetActive(false);
            yield return new WaitForSecondsRealtime(timeToSpawn);
            _spawnedPlayerObject.SetActive(true);
            _spawnCache = null;
        }
    }
}
