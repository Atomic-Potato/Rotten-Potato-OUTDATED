using UnityEngine;

public abstract class Enemy : MonoBehaviour, IParriable
{
    public abstract void Die();    
    public abstract void Damage();

    public abstract void Attack();

    public abstract bool IsParriable();
    public abstract void Parry();
}
