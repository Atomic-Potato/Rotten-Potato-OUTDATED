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
            if (Dash.IsDashing)
            {
                return;
            }

            Dash dash = other.gameObject.GetComponent<Dash>();

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
        Player.Instance.Damage?.Invoke(damage);
    }

    void KnockPlayerBack(Dash dash)
    {
        Vector2 direction = (Player.Instance.transform.position - transform.position).normalized;
        dash.ApplyDashForce(true, direction, knockBackTime, knockBackDistance);
    }

    public override bool IsParriable()
    {
        return false;
    }

    public override void Parry()
    {
    }
}
