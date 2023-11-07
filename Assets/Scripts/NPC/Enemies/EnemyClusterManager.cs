using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

public class EnemyClusterManager : MonoBehaviour
{
    [Range(0, 20)]
    [SerializeField] int enemiesCount; // Limited to 20 due to floating point imprecision
    [Tooltip("Enemies will spawn in a circle pattern with this radius")]
    [Range(0f, 100f)]
    [SerializeField] float radius;
    [Tooltip("Rotates enemies around the center")]
    
    [Space]
    [SerializeField] bool rotate;
    [SerializeField] float rotationSpeed;

    [Space]
    [Tooltip("The type of enemy to be spawned.")]
    [SerializeField] GameObject enemy;
    [Tooltip("The parent object of the enemies")]
    [SerializeField] GameObject parent;

    float theta;
    Vector2 a;
    Vector2 b;
    float currentAngle;
    bool _isSpawnedEnemies;

   void Awake()
    {
        theta = 360 / enemiesCount * Mathf.Deg2Rad;
        a.x = Mathf.Cos(theta);
        a.y = Mathf.Sin(theta);
        b.x = -Mathf.Sin(theta);
        b.y = Mathf.Cos(theta);
    }

    void Start() 
    {
        SpawnEnemies();   
    }

    void Update()
    {
        if (!_isSpawnedEnemies)
        {
            return;
        }

        RotateParent();
    }

    public void SpawnEnemies()
    {
        // NOTE:
        // This method suffers from floating point precision inacuraccy
        // So the enemiesCount was limited to 20

        _isSpawnedEnemies = true;

        Vector2 position = new Vector2(1f, 0) * radius;
        for (int i=0; i < enemiesCount; i++)
        {
            Spawn();
            position = RotateVectorByTheta(position);
        }

        Vector2 RotateVectorByTheta(Vector2 v)
        {
            return a * v.x + b * v.y;
        }

        void Spawn()
        {
            GameObject spawned = Instantiate(enemy, (Vector2)parent.transform.position + position, Quaternion.identity, parent.transform);
            EnemyProjectileShooting shooting = spawned.GetComponent<EnemyProjectileShooting>();
            shooting.target = transform;
        }
    }


    void RotateParent()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
