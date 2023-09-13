using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Range(0f, 5f)]
    [SerializeField] float timeToCounterAttack;
    
    [Space(height: 10)]
    [SerializeField] EnemyPathManager pathManager;
    [SerializeField] SpriteRenderer spriteRenderer;

    public float TimeToCounterAttack => timeToCounterAttack;

    float _counterAttackTimer;
    bool _isCanCounterAttack;
    bool _isCounterAttacking;
    bool _isCanAttack;
    bool _isAttacking;
    Color _color;

    public Transform origin;

    private void OnDrawGizmos() {
        Gizmos.color = Color.white;
        Vector2 dir = (transform.position - origin.transform.position).normalized;
        Gizmos.DrawRay(origin.transform.position, dir * Vector2.Distance(transform.position, origin.transform.position));
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            pDash dash = other.gameObject.GetComponent<pDash>();

            if (dash.IsDashing)
            {
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
                
                _isCanCounterAttack = true;
            }
            else
            {
                _isCanAttack = true;
            }
        }    
    }

    void Update()
    {
        if (_isCanCounterAttack)
        {
            CounterAttack();
        }
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
        }
    }

    void StopCounterAttack(Color color)
    {
        _isCanCounterAttack = false;
        _isCounterAttacking = false;
        _counterAttackTimer = 0f;
        spriteRenderer.color = color;
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
