using System.Collections;
using UnityEngine;

public class EnemyProjectileShooting : MonoBehaviour
{
    [SerializeField] float timeBetweenShots;
    [SerializeField] bool shootOnStart;
    [Tooltip("Negative distance means infinite distance.")]
    [Range(-1f, 100f)]
    [SerializeField] float distanceToAttack = 10f;
    [SerializeField] GameObject projectile;
    [Tooltip("If left null, the target will be the player.")]
    public Transform target;

    Coroutine _shootingCache = null;

    void Awake()
    {
        if (target == null)
        {
            target = Player.Instance.transform;
        }
    }

    void OnDisable() 
    {
        if (_shootingCache != null)
        {
            StopCoroutine(_shootingCache);
            _shootingCache = null;
        }
    }

    void Update() 
    {
        if (_shootingCache == null && (distanceToAttack < 0f || IsTargetWithingAttackDistance()))
        {
            _shootingCache = StartCoroutine(Shoot());
        }

        bool IsTargetWithingAttackDistance()
        {
            return Vector2.Distance(target.position, transform.position) < distanceToAttack;
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
        Instantiate(projectile, transform.position, GetRotationToTarget());

        Quaternion GetRotationToTarget()
        {
            Vector2 res = target.position - transform.position;
            float angle = Mathf.Atan2(res.y, res.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}
