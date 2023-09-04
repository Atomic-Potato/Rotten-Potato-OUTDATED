using System.Collections;
using UnityEngine;

public class pBulletController : MonoBehaviour
{
    [SerializeField] float launchForce = 2f;
    [Range(0f, 30f)]
    [SerializeField] float lifeTime = 10f;

    [Space]
    [SerializeField] Rigidbody2D rigidbody;

    void OnEnable()
    {
        rigidbody.AddForce(Vector2.right * launchForce, ForceMode2D.Impulse);

        StartCoroutine(SelfDestructInTime());
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            DamagePlayer();
            SelfDestroy();
        }
        else
        {
            SelfDestroy();
        }
    }

    void DamagePlayer()
    {
        
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
