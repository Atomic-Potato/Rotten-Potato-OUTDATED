using System.Collections;
using UnityEngine;

public class pBulletController : MonoBehaviour
{
    [Range(0, 999)]
    [SerializeField] int damagePoints;
    [SerializeField] float launchForce = 2f;
    [Range(0f, 30f)]
    [SerializeField] float lifeTime = 10f;

    [Space]
    [SerializeField] Rigidbody2D rigidbody;

    void OnEnable()
    {
        rigidbody.AddRelativeForce(Vector2.right * launchForce, ForceMode2D.Impulse);

        StartCoroutine(SelfDestructInTime());
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            DamagePlayer();
            SelfDestroy();
        }
        else if (other.gameObject.tag != "Projectile")
        {
            SelfDestroy();
        }
    }

    void DamagePlayer()
    {
        pPlayer.Instance.Damage?.Invoke(damagePoints);
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }

    IEnumerator SelfDestructInTime()
    {
        yield return new WaitForSeconds(lifeTime);
        SelfDestroy();
    }
}
