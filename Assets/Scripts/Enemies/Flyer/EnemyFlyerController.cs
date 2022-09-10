using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyFlyerController : MonoBehaviour
{
    [SerializeField] int health = 25;
    [Tooltip("The distance at which the object stops moving")]
    [SerializeField] float enterDistance = 5f;
    [Tooltip("The distance at which the object starts moving again if its inside the \"EnterDistance\"")]
    [SerializeField] float exitDistance = 8f;
    [Tooltip("Amount of bullets to shoot at the same time")]
    [SerializeField] int amountOfBullets = 3;
    [Tooltip("The fire rate at which the enemy shoots. Can't be 0")]
    [SerializeField] float fireRate = 1f;
    [Tooltip("The angle between each bullet fired")]
    [SerializeField] float bulletDispersion = 20f;
    [SerializeField] string playerObjectName;

    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] GameObject flyerBullet;
    [SerializeField] Transform mouthTransform;
    [SerializeField] AIPath aiPath;

    [HideInInspector] public bool gotKnocked;

    int bulletsLeft;
    int bulletPos;

    float oldAIspeed;
    float timeToFire;
    float generalTimer = 0f;

    bool valueGiven;

    Transform playerTransform;

    void Start()
    {
        playerTransform = GameObject.Find(playerObjectName).GetComponent<Transform>();
        //rocketController = GameObject.FindGameObjectWithTag("Rocketato").GetComponent<RocketatoController>();
        bulletsLeft = amountOfBullets;
        bulletPos = amountOfBullets / 2;
        oldAIspeed = aiPath.maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        StopMovement();

        if (gotKnocked)
            StopKnock(RocketatoController.timeToDisableKnock, 0f); //This function is also used for the player knock, because we dont really need to change the time to restore flyer defaults
        if (valueGiven)
            generalTimer -= Time.deltaTime;
    }

    void StopMovement()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance < enterDistance)
        {
            aiPath.maxSpeed = 0f;

            //SHOOTING
            if (Time.time > timeToFire) //Time.time is the current time
            {
                /*When in range, it shoots once, 
                and then create this variable which was the time the bullet
                got shot plus the delay then it waits this too shoot again and so on*/

                timeToFire = Time.time + 1 / fireRate;
                Shoot();
            }
        }
        else if (distance > exitDistance)
        {
            aiPath.maxSpeed = oldAIspeed;
        }
    }

    void Shoot()
    {
        Vector3 targetPos = PlayerController.player.transform.position;
        targetPos.z = 0f;

        targetPos.x -= mouthTransform.position.x;
        targetPos.y -= mouthTransform.position.y;

        float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;

        //In here we handle amount of bullets to shoot and at which angle to shoot each bullet
        //To decide the position we just get the amount of bullets and divide it by 2
        //Before i go further, at the angle, the middle bullet will be at position 0
        //Say we shoot 3 bullet, 1st bullet at pos 1, 2nd at 0, 3rd at -1
        //For odd bullets when we reach pos 0 we reduce the position again so it wouldnt shoot one at the center and one on the side, and so both on the sides if we remove the zero pos
        while (bulletsLeft > 0)
        {
            Instantiate(flyerBullet, mouthTransform.position, Quaternion.Euler(new Vector3(0f, 0f, angle - 90f - bulletPos * bulletDispersion))); // its 90 degrees off for some reason

            bulletPos--;

            if(bulletPos == 0 && amountOfBullets % 2 == 0)
                bulletPos--;

            bulletsLeft--;
        }

        bulletPos = amountOfBullets / 2;
        bulletsLeft = amountOfBullets;
    }

    public void GetDamaged(int damage)
    {
        health -= damage;

        if (health <= 0)
            Destroy(gameObject);
    }

    //This function is used when the flyer gets knocked by a rocketato
    //Its called in here and not in the rocket controller in case of multiple enemies being hit at once
    //the rocketato cant run a timer for each one and enable back their scripts
    //so instead each flyer enables back itself
    public void StopKnock(float time, float finalGravity)
    {
        //This if statement is used to stop the timer from resetting while the condition for this function is true
        if (!valueGiven)
        {
            generalTimer = time;
            valueGiven = true;
        }


        if (generalTimer <= 0)
        {
            //Resetting flyer old values
            rigidBody.drag = 0f;
            rigidBody.gravityScale = finalGravity;

            //Enabling the flyer scripts
            
            gameObject.GetComponent<AIPath>().enabled = true;
            gameObject.GetComponent<AIDestinationSetter>().enabled = true;

            valueGiven = false;
            gotKnocked = false;
        }
    }
}
