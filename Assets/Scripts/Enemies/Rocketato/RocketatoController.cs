using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class RocketatoController : MonoBehaviour
{
    public float speed = 10f;
    public float maxSpeed = 20f;
    [Space]
    [SerializeField] float enemyKnockLinearDrag = 1f;
    public static float timeToDisableKnock = 1f;
    [SerializeField] float enemyKnockGravityForce = 7f;
    [Space]
    [SerializeField] Vector2 playerKnockForce;
    [SerializeField] Vector2 enemyKnockForce;

    [Space]
    [Space]
    
    public Rigidbody2D rigidBody;


    float originalSpeed;

    private void Start()
    {
        speed *= (-1);
        maxSpeed *= (-1);
    }

    private void FixedUpdate()
    {
        Move(speed);
    }

    private void Move(float velocity)
    {
        rigidBody.velocity = new Vector2(0f, velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        originalSpeed = speed;

        //Slow to half of the current speed when inside ground
        if (collider.gameObject.CompareTag("Ground"))
            speed = speed / 2;
        
        if (collider.gameObject.CompareTag("InstaKill"))
            Destroy(gameObject);

        Knockback(collider);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        //Slow to half of the current speed when inside ground
        if (collider.gameObject.CompareTag("Ground"))
            speed = originalSpeed;
    }

    public void OnHitEffect(float speedToAdd)
    {
        if(speed > maxSpeed) //Its greater because we're working on the negative axis
        {
            speed -= speedToAdd;
            speed = Mathf.Max(speed, maxSpeed);
        }
    }

    void Knockback(Collider2D collider)
    {
        if (collider.CompareTag("Flyer"))
        {
            Rigidbody2D foundRigidBody = collider.GetComponent<Rigidbody2D>();

            //Disabling scripts that can interfer with the force
            collider.GetComponent<AIPath>().enabled = false;
            collider.GetComponent<AIDestinationSetter>().enabled = false;

            //Deciding the direction of the force
            if (transform.position.x < collider.GetComponent<Transform>().position.x)
                foundRigidBody.AddForce(enemyKnockForce, ForceMode2D.Impulse);
            else
                foundRigidBody.AddForce(new Vector2(enemyKnockForce.x * -1, enemyKnockForce.y), ForceMode2D.Impulse);

            foundRigidBody.drag = enemyKnockLinearDrag;
            foundRigidBody.gravityScale = enemyKnockGravityForce;

            collider.GetComponent<EnemyFlyerController>().gotKnocked = true;
        }


        if (collider.CompareTag("Player"))
        {
            Rigidbody2D playerRigidBody = collider.GetComponent<Rigidbody2D>();

            //If the rocketato knocks the player on the ground he wount move because of the full friction
            PlayerController.isKnocked = true;

            //Deciding the direction of the force
            if (transform.position.x < collider.GetComponent<Transform>().position.x)
                playerRigidBody.AddForce(playerKnockForce * 100f, ForceMode2D.Force);
            else
                playerRigidBody.AddForce(new Vector2(playerKnockForce.x * -1f * 100f, playerKnockForce.y), ForceMode2D.Force);
        }
    }
}
