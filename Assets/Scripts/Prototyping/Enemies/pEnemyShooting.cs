using System.Collections;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

public class pEnemyShooting : MonoBehaviour
{
    [SerializeField] float timeBetweenShots;
    [SerializeField] bool shootOnStart;
    [SerializeField] float distanceToAttack = 10f;
    [SerializeField] GameObject bulletObject;

    Coroutine _shootingCache = null;

    void OnEnable()
    {
        if (_shootingCache != null)
        {
            StopCoroutine(_shootingCache);
            _shootingCache = null;
        }    
    }

    void Update() 
    {
        if (_shootingCache == null && IsPlayerWithingAttackDistance())
        {
            _shootingCache = StartCoroutine(Shoot());
        }

        bool IsPlayerWithingAttackDistance()
        {
            return Vector2.Distance(pPlayer.Instance.Object.transform.position, transform.position) < distanceToAttack;
        }
    }

    IEnumerator Shoot()
    {
        if (shootOnStart)
        {
            LaunchBullet();
        }

        yield return new WaitForSeconds(timeBetweenShots);

        if (!shootOnStart)
        {
            LaunchBullet();
        }

        _shootingCache = null;
    }

    void LaunchBullet()
    {
        Instantiate(bulletObject, transform.position, GetRotationToPlayer());

        Quaternion GetRotationToPlayer()
        {
            Vector2 res = pPlayer.Instance.Object.transform.position - transform.position;
            float angle = Mathf.Atan2(res.y, res.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}
