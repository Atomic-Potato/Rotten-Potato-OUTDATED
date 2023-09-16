using UnityEngine;

public class EnemyClusterManager : MonoBehaviour
{
    [Range(0, 100)]
    [SerializeField] int enemiesCount;
    [Tooltip("Enemies will spawn in a circle pattern with this radius")]
    [Range(0f, 100f)]
    [SerializeField] float radius;
    [Tooltip("Rotates enemies around the center")]
    [SerializeField] bool rotate;

    [Space]
    [Tooltip("The type of enemy to be spawned.")]
    [SerializeField] Enemy enemy;
}
