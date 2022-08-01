using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
#pragma warning disable 0649
    public int playerBulletDamage = 10;
    public int companionBulletDamage = 5;
    public float speed;

    [SerializeField] float lifeTime;
    [Tooltip("How much to add to the Rocketato speed when it gets hit with a bullet")]
    [SerializeField] float rocketSpeedAddition = 10f;
    [Space]
    [SerializeField] string groundTag;
    [SerializeField] string flyerTag;

    Transform playerTransfom;
    

    void Start()
    {
        playerTransfom = PlayerController.player.transform;

        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D colInfo)
    {
        //Player Bullets
        //Collision with ground
        if (colInfo.gameObject.CompareTag(groundTag) && (gameObject.tag == "Player Bullet" || gameObject.tag == "Companion Bullet"))
            Destroy(gameObject);

        //Collision with Enemy Flyer
        if (colInfo.gameObject.CompareTag(flyerTag) && gameObject.tag == "Player Bullet")
        {
            Destroy(gameObject);
            colInfo.gameObject.GetComponent<EnemyFlyerController>().GetDamaged(playerBulletDamage);
        }

        //Collision with Rocketato
        if (colInfo.gameObject.CompareTag("Rocketato") && (gameObject.tag == "Player Bullet" || gameObject.tag == "Companion Bullet"))
        {
            Destroy(gameObject);
            colInfo.gameObject.GetComponent<RocketatoController>().OnHitEffect(rocketSpeedAddition);
        }

        //Companion Bullets
        if (colInfo.gameObject.CompareTag(flyerTag) && gameObject.tag == "Companion Bullet")
        {
            Destroy(gameObject);
            colInfo.gameObject.GetComponent<EnemyFlyerController>().GetDamaged(companionBulletDamage);
        }


        //Enemy Bullets
        if (colInfo.gameObject.CompareTag("Player") && gameObject.tag == "Flyer")
            Destroy(gameObject);
    }
}
