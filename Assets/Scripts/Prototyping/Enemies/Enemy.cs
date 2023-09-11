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
                LinkedPoint point = pathManager.MoveToNextPoint();
                if (point == null)
                {
                    Die();
                    return;
                }
                
                if (point.type == LinkedPoint.Types.Random)
                {
                    spriteRenderer.color = Color.red;
                }
                else if (point.type == LinkedPoint.Types.Linear)
                {
                    spriteRenderer.color = Color.green;
                }
                //_isCanCounterAttack = true;
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
        if (_isCanCounterAttack)
        {
            _isCounterAttacking = true;
            _counterAttackTimer = 0f;
        }

        if (_counterAttackTimer >= timeToCounterAttack)
        {
            pathManager.MoveToPreviousPoint();

            _isCanCounterAttack = false;
            _isCounterAttacking = false;
        }
    }
}
