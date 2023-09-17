using UnityEngine;

public class MediumEnemy : Enemy, IParriable
{
    [Space]
    [SerializeField] int damageToPlayer;
    [Tooltip("If no previous path points are found, then this is the amount of distance the player will be knocked.")]
    [SerializeField] float knockBackDistance;
    
    [Space]
    [Range(0, 1f)]
    [SerializeField] float counterAttackProbability;
    [Tooltip("The probability of the enemy to keep counter attacking after the last counter attack.")]
    [Range(0f, 1f)]
    [SerializeField] float keepCounterAttackingProbability;
    [Range(0f, 1f)]
    [Tooltip("The probability of the enemy attacking the player isntead of getting damaged.")]
    [SerializeField] float attackPlayerProbability;
    [Range(0f, 20f)]
    [SerializeField] float timeToCounterAttack;
    [Range(0f, 20f)]
    [SerializeField] float timeToBeParried;
    [Tooltip("If enabled, allows the enemy to counter attack until the start of the current section it took."
        + " \nIf disabled, will counter attack until the start of the entire path.")]
    [SerializeField] bool isCanCounterToSectionStartOnly;

    [Space]
    [Tooltip("The cluster to be spawned after the enemy death.")]
    [SerializeField] GameObject enemyCluster;

    [Space(height: 10)]
    [SerializeField] EnemyPathManager pathManager;
    [SerializeField] EnemyProjectileShooting shooting;
    [SerializeField] SpriteRenderer spriteRenderer;

    [Space]
    [Header("Gizmos & Debugging")]
    [SerializeField] Color parryColor;
    public float TimeToCounterAttack => timeToCounterAttack;

    float _counterAttackTimer;
    float _toBeParriedTimer;
    bool _isCanCounterAttack;
    bool _isCounterAttacking;
    bool _isCanAttack;
    bool _isAttacking;
    bool _isParriable;
    bool _isPlayerInRange;

    pDash _playerDash;
    GameObject _spawnedCluster;

    [SerializeField] Transform origin;

    #region Execution
    void Awake()
    {
        _spawnedCluster = Instantiate(enemyCluster);
        _spawnedCluster.SetActive(false);
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.white;
        Vector2 dir = (transform.position - origin.transform.position).normalized;
        Gizmos.DrawRay(origin.transform.position, dir * Vector2.Distance(transform.position, origin.transform.position));
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            _isPlayerInRange = true;
            if (_playerDash == null)
            {
                _playerDash = other.gameObject.GetComponent<pDash>();
            }
        }    
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            _isPlayerInRange = false;
        }    
    }

    void Update()
    {
        shooting.enabled = !_isCounterAttacking && !_isAttacking ? true : false; 

        if (_isCanCounterAttack)
        {
            CounterAttack();
        }
        else if (_isCanAttack)
        {
            Attack();
        }
    }
    #endregion

    public override void Damage()
    {
        if(_isAttacking)
        {
            return;
        }

        if (_isCanCounterAttack)
        {
            StopCounterAttack(Color.white);
        }
        
        _isCanAttack = RollForSuccess(attackPlayerProbability);
        if (_isCanAttack)
        {
            return;
        }

        LinkedPoint point = pathManager.MoveToNextPoint();
        if (point == null)
        {
            Die();
            return;
        }
        spriteRenderer.color = GetPointColor(point);

        _isCanCounterAttack = RollForSuccess(counterAttackProbability);
    }

    void DamageNoAttack()
    {
        if(_isAttacking)
        {
            return;
        }

        if (_isCanCounterAttack)
        {
            StopCounterAttack(Color.white);
        }
        
        LinkedPoint point = pathManager.MoveToNextPoint();
        if (point == null)
        {
            Die();
            return;
        }
        spriteRenderer.color = GetPointColor(point);

        _isCanCounterAttack = RollForSuccess(counterAttackProbability);
    }

    public override void Die()
    {
        _spawnedCluster.SetActive(true);
        _spawnedCluster.transform.position = transform.position;
        Destroy(gameObject);
    }

    #region Parrying
    public override bool IsParriable()
    {
        return _isParriable;
    }
    public override void Parry()
    {
        if (!_isParriable)
        {
            return;
        }
        
        if (_isAttacking)
        {
            StopAttack(Color.white);
        }
        _isParriable = false;
        
        DamageNoAttack();
    }
    #endregion

    #region Counter Attack
    void CounterAttack()
    {
        if (isCanCounterToSectionStartOnly)
        {
            int currentPointIndex = pathManager.GetCurrentPointIndex();
            if (currentPointIndex <= 0)
            {
                StopCounterAttack(GetPointColor(pathManager.GetCurrentPoint()));
                return;
            }
        }

        if (!_isCounterAttacking)
        {
            _isCounterAttacking = true;
            _counterAttackTimer = 0f;
            spriteRenderer.color = Color.yellow;
        }

        _counterAttackTimer += Time.deltaTime;

        if (_counterAttackTimer >= timeToCounterAttack)
        {
            LinkedPoint point = pathManager.MoveToPreviousPoint();
            StopCounterAttack(GetPointColor(point));
            _isCanAttack = true;
        }
    }

    void StopCounterAttack(Color color)
    {
        _isCanCounterAttack = false;
        _isCounterAttacking = false;
        _counterAttackTimer = 0f;
        spriteRenderer.color = color;
    }
    #endregion

    #region Attack
    public override void Attack()
    {
        if (!_isAttacking)
        {
            _isAttacking = true;
            _isParriable = true;
            _toBeParriedTimer = 0f;
            Color pink = new Color(255,105,180, 1);
            spriteRenderer.color = parryColor;
        }

        _toBeParriedTimer += Time.deltaTime;

        if (_toBeParriedTimer >= timeToBeParried)
        {
            if (_isPlayerInRange)
            {
                pPlayer.Instance.Damage?.Invoke(damageToPlayer);
                if (pDash.IsHolding)
                {
                    KnockPlayerBack();
                }
            }

            StopAttack(GetPointColor(pathManager.GetCurrentPoint()));

            bool isShouldCounterAttackAgain = RollForSuccess(keepCounterAttackingProbability);
            if (isShouldCounterAttackAgain)
            {
                _isCanCounterAttack = true;
            }
        }

        void KnockPlayerBack()
        {
            LinkedPoint previousPoint = pathManager.GetCurrentPoint().Previous;
            Vector2 direction;
            float distance;

            if (previousPoint != null)
            {
                direction = (previousPoint.position - (Vector2)pPlayer.Instance.transform.position).normalized;
                distance = Vector2.Distance(pPlayer.Instance.transform.position, previousPoint.position);
                _playerDash.Dash(true, direction, 0.1f, distance);
            }
            else
            {
                direction = pPlayer.Instance.transform.position - transform.position;
                _playerDash.Dash(true, direction, 0.1f, knockBackDistance);
            }
        }
    }

    void StopAttack(Color color)
    {
        _isCanAttack = false;
        _isAttacking = false;
        _toBeParriedTimer = 0f;
        spriteRenderer.color = color;
    }
    #endregion

    #region Methods
    bool RollForSuccess(float probablityOfSuccess)
    {
        float random = Random.Range(0f, 1f);
        return random <= probablityOfSuccess;
    }

    Color GetPointColor(LinkedPoint point)
    {
        if (point.type == LinkedPoint.Types.Random)
        {
            return Color.red;
        }
        else if (point.type == LinkedPoint.Types.Linear)
        {
            return Color.green;
        }
        else
        {
            return Color.white;
        }
    }
    #endregion
}
