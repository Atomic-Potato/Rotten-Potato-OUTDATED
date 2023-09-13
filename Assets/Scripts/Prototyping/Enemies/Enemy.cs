using TMPro;
using UnityEditor.U2D;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Space]
    [SerializeField] int damage;

    [Space]
    [Range(0, 1f)]
    [SerializeField] float counterAttackProbability;
    [Range(0f, 20f)]
    [SerializeField] float timeToCounterAttack;
    [Range(0f, 20f)]
    [SerializeField] float timeToBeParried;
    
    [Space(height: 10)]
    [SerializeField] EnemyPathManager pathManager;
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
    public bool IsAttacking;
    bool _isPlayerInRange;

    pDash _playerDash;

    [SerializeField] Transform origin;

    private void OnDrawGizmos() {
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
        if (_isCanCounterAttack)
        {
            CounterAttack();
        }
        else if (_isCanAttack)
        {
            Attack();
        }
    }

    public void Damage()
    {
        if(IsAttacking)
        {
            return;
        }

        if (_isCanCounterAttack)
        {
            StopCounterAttack(Color.white);
        }

        LinkedPoint point = pathManager.MoveToNextPoint();
        spriteRenderer.color = GetPointColor(point);
        if (point == null)
        {
            Die();
            return;
        }

        _isCanCounterAttack = RollForSuccess(counterAttackProbability);
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    void CounterAttack()
    {
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

    void Attack()
    {
        if (!IsAttacking)
        {
            IsAttacking = true;
            _toBeParriedTimer = 0f;
            Color pink = new Color(255,105,180, 1);
            spriteRenderer.color = parryColor;
        }

        _toBeParriedTimer += Time.deltaTime;

        if (_toBeParriedTimer >= timeToBeParried)
        {
            if (_isPlayerInRange)
            {
                pPlayer.Instance.Damage?.Invoke(damage);
                if (pDash.IsHolding)
                {
                    KnockPlayerBack();
                }
            }

            StopAttack(GetPointColor(pathManager.GetCurrentPoint()));
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
                distance = 1f;
                _playerDash.Dash(true, direction, 0.1f, distance);
            }
        }
    }

    void StopAttack(Color color)
    {
        _isCanAttack = false;
        IsAttacking = false;
        _toBeParriedTimer = 0f;
        spriteRenderer.color = color;
    }

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
}
