using System.Collections;
using UnityEngine;

public class SmallEnemy : Enemy
{
    [Space]
    [Tooltip("The damage points applied to player upon collision.")]
    [SerializeField] int damage;
    [SerializeField] float knockBackDistance = 2.5f;
    [Range(0.1f, 5f)]
    [SerializeField] float knockBackTime = 0.15f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (pDash.IsDashing)
            {
                return;
            }

            pDash dash = other.gameObject.GetComponent<pDash>();

            Attack();
            KnockPlayerBack(dash);
        }
    }
    
    public override void Damage()
    {
        Die();
    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    public override void Respawn()
    {
        throw new System.NotImplementedException();
    }

    public override void Attack()
    {
        pPlayer.Instance.Damage?.Invoke(damage);
    }

    void KnockPlayerBack(pDash dash)
    {
        Vector2 direction = (pPlayer.Instance.transform.position - transform.position).normalized;
        dash.Dash(true, direction, knockBackTime, knockBackDistance);
    }

    public override bool IsParriable()
    {
        return false;
    }

    public override void Parry()
    {
    }
}
