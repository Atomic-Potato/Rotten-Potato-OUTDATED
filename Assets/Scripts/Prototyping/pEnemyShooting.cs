using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class pEnemyShooting : MonoBehaviour
{
    [SerializeField] int timeBetweenShots;
    [SerializeField] bool shootOnStart;
    [SerializeField] GameObject bulletObject;

    IEnumerator Shoot()
    {
        if (shootOnStart)
        {

        }
    }

    void LaunchBullet()
    {
        Instantiate(bulletObject, transform.position, quaternion.identity);
    }
}
