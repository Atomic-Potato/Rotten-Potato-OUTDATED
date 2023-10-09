using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    [Tooltip("Make sure that no other checkpoints have this enabled." + 
        "Otherwise it will almost random which point is chosen.")]
    [SerializeField] bool isLevelStartingPoint;
    [SerializeField] Transform spawnPosition;

    void Awake()
    {
        if (isLevelStartingPoint)
        {
            SpawnManager.Instance.LevelSpawnPoint = spawnPosition.position;
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == TagsManager.Tag_Player)
        {
            SpawnManager.Instance.Checkpoint = spawnPosition.position;
        }    
    }
}
