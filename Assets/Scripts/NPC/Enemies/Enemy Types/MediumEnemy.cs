using System;
using System.Collections;
using System.Linq.Expressions;
using Unity.Mathematics;
using UnityEngine;

public class MediumEnemy : Enemy, IParriable
{
    #region Inspector
    [Space]
    [Header("Respawning")]
    [SerializeField] bool isShouldRespawn;
    [SerializeField] float timeToRespawn;

    [Space]
    [Header("Damage")]
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
    [SerializeField] Collider2D collider;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject sprite;
    [SerializeField] GameObject deathEffect;

    [Space]
    [Header("Audio")]
    [SerializeField] AudioSource audioAttack;
    [SerializeField] AudioSource audioDeath;
    [SerializeField] AudioSource audioTeleport;
    

    [Space]
    [Header("Gizmos & Debugging")]
    [SerializeField] Color parryColor;
    #endregion

    #region Global Variables
    int _hitPoints;
    public int HitPoints => _hitPoints;
    public Action UpdateHitPoints;

    float _counterAttackTimer;
    public float CounterAttackTimer => _counterAttackTimer;
    public float CounterAttackTime => timeToCounterAttack;
    float _toBeParriedTimer;
    public float ToBeParriedTimer => _toBeParriedTimer;
    public float ToBeParriedTime => timeToBeParried;
    bool _isCanCounterAttack;
    bool _isCounterAttacking;
    public bool IsCounterAttacking => _isCounterAttacking;

    bool _isCanAttack;
    bool _isAttacking;
    public bool IsAttacking => _isAttacking;
    
    bool _isParriable;
    bool _isPlayerInRange;
    bool _isRespawning;


    Color _previousColor;
    Dash _playerDash;
    GameObject _spawnedCluster;
    Coroutine _respawnCache;
    [SerializeField] Transform origin;
    #endregion

    #region Execution
    void Awake()
    {
        if (enemyCluster != null)
        {
            _spawnedCluster = Instantiate(enemyCluster);
            _spawnedCluster.SetActive(false);
        }

        UpdateHitPoints = ExecuteUpdateHitPoints;
        UpdateHitPoints.Invoke();
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
                _playerDash = other.gameObject.GetComponent<Dash>();
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
        if (shooting != null)
        {
            shooting.enabled = !_isCounterAttacking && !_isAttacking && !_isRespawning ? true : false; 
        }

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
        UpdateHitPoints.Invoke();
        if (point == null)
        {
            if (isShouldRespawn)
            {
                Respawn();
            }
            else
            {
                Die();
            }
            return;
        }
        if (point != null)
        {
            spriteRenderer.color = (Color)GetPointColor(point);
        }
        AudioManager.PlayAudioSource(audioTeleport);

        _isCanCounterAttack = point.Previous != null && RollForSuccess(counterAttackProbability);
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
        UpdateHitPoints.Invoke();
        if (point == null)
        {
            if (isShouldRespawn)
            {
                Respawn();
            }
            else
            {
                Die();
            }
            return;
        }
        if (point != null)
        {
            spriteRenderer.color = (Color)GetPointColor(point);
        }
        AudioManager.PlayAudioSource(audioTeleport);

        _isCanCounterAttack = RollForSuccess(counterAttackProbability);
    }

    public override void Die()
    {
        if (enemyCluster != null)
        {
            _spawnedCluster.SetActive(true);
            _spawnedCluster.transform.position = transform.position;
        }
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        AudioManager.PlayAudioSource(audioDeath);
        Destroy(gameObject);
    }

    public override void Respawn()
    {
        if (_respawnCache == null)
        {
            _respawnCache = StartCoroutine(ExecuteRespawn());
        }

        IEnumerator ExecuteRespawn()
        {
            if (enemyCluster != null)
            {
                _spawnedCluster.SetActive(true);
                _spawnedCluster.transform.position = transform.position;
            }
            AudioManager.PlayAudioSource(audioDeath);

            Hide();
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            _isRespawning = true;
            pathManager.Reset();
            LinkedPoint point = pathManager.MoveToNextPoint();
            UpdateHitPoints.Invoke();
            if (point != null)
            {
                spriteRenderer.color = (Color)GetPointColor(point);
            }
            yield return new WaitForSeconds(timeToRespawn);
            Show();
            _isRespawning = false;
            _respawnCache = null;
        }

        void Hide()
        {
            if(shooting != null)
            {
                shooting.enabled = false;
            }
            sprite.SetActive(false);
            collider.enabled = false;
        }

        void Show()
        {
            if(shooting != null)
            {
                shooting.enabled = true;
            }
            sprite.SetActive(true);
            collider.enabled = true;
            UpdateHitPoints();
        }
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
            _previousColor = spriteRenderer.color;
            spriteRenderer.color = Color.yellow;
        }

        _counterAttackTimer += Time.deltaTime;

        if (_counterAttackTimer >= timeToCounterAttack)
        {
            LinkedPoint point = pathManager.MoveToPreviousPoint();
            UpdateHitPoints.Invoke();
            StopCounterAttack(GetPointColor(point));
            if (point == null)
            {
                spriteRenderer.color = _previousColor;
            }
            else
            {
                AudioManager.PlayAudioSource(audioTeleport);
                _isCanAttack = true;
            }
        }
    }

    void StopCounterAttack(Color? color = null)
    {
        _isCanCounterAttack = false;
        _isCounterAttacking = false;
        _counterAttackTimer = 0f;
        if (color != null)
        {
            spriteRenderer.color = (Color)color;
        }
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
                Player.Instance.Damage?.Invoke(damageToPlayer);
                if (Dash.IsHolding)
                {
                    KnockPlayerBack();
                }
            }
            
            AudioManager.PlayAudioSource(audioAttack);
            StopAttack(GetPointColor(pathManager.GetCurrentPoint()));

            LinkedPoint currentPoint = pathManager.GetCurrentPoint();
            if (currentPoint != null && currentPoint.Previous != null)
            {
                bool isShouldCounterAttackAgain = RollForSuccess(keepCounterAttackingProbability);
                if (isShouldCounterAttackAgain)
                {
                    _isCanCounterAttack = true;
                }
            }
        }

        void KnockPlayerBack()
        {
            LinkedPoint previousPoint = pathManager.GetCurrentPoint()?.Previous;
            Vector2 direction;
            float distance;

            if (previousPoint != null)
            {
                direction = (previousPoint.position - (Vector2)Player.Instance.transform.position).normalized;
                distance = Vector2.Distance(Player.Instance.transform.position, previousPoint.position);
                _playerDash.ApplyDashForce(true, direction, 0.1f, distance);
            }
            else
            {
                direction = Player.Instance.transform.position - transform.position;
                _playerDash.ApplyDashForce(true, direction, 0.1f, knockBackDistance);
            }
        }
    }

    void StopAttack(Color? color = null)
    {
        _isCanAttack = false;
        _isAttacking = false;
        _toBeParriedTimer = 0f;
        if (color != null)
        {
            spriteRenderer.color = (Color)color;
        }
    }
    #endregion

    #region Methods
    bool RollForSuccess(float probablityOfSuccess)
    {
        float random = UnityEngine.Random.Range(0f, 1f);
        return random <= probablityOfSuccess;
    }

    Color? GetPointColor(LinkedPoint point)
    {
        if (point == null)
        {
            return null;
        }

        if (point.type == LinkedPoint.Types.Random)
        {
            return Color.red;
        }
        else if (point.type == LinkedPoint.Types.Linear)
        {
            return Color.red;
        }
        
        return null;
    }

    void ExecuteUpdateHitPoints()
    {
        if (pathManager == null)
            return;
        int length = pathManager.GetCurrentSectionLength();
        int index = pathManager.GetCurrentPointIndex();
        _hitPoints =  length - index + 1;
    }
    #endregion
}
