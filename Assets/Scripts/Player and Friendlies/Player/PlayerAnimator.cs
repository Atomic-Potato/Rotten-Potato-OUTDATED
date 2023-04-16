using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
#pragma warning disable 0649
    //Publics
    [Header("Sprite Animations")]
    [SerializeField] Animator animator;

    [Space]
    [Header("Calculated Animations")]

    //Components
    [SerializeField] SpriteRenderer playerSprite;

    //Scripts
    [SerializeField] PlayerController playerScript;
    
    [Space]

    [SerializeField] GameObject WalkingRightParticles;
    [SerializeField] GameObject WalkingLeftParticles;
    [SerializeField] GameObject DashingRightParticles;
    [SerializeField] GameObject DashingLeftParticles;
    [SerializeField] GameObject DashingUpParticles;
    [SerializeField] GameObject DashingDownParticles;
    
    Vector3 oldPos;
    Vector3 newPos;

    [HideInInspector] public float distanceTraveled;

    void Start()
    {
        oldPos = transform.position;
    }

    void Update()
    {
        SpriteFlipping();
        playerLastPos();
        MovementAnimations();
        MovementParticles();
    }

    void MovementAnimations() 
    {
        if (playerScript.rigidBody.velocity.x > 0)
        {
            animator.SetBool("isGoingRight", true);
            animator.SetBool("isGoingLeft", false);
        }
        else if (playerScript.rigidBody.velocity.x < 0)
        {
            animator.SetBool("isGoingRight", false);
            animator.SetBool("isGoingLeft", true);
        }

        if (playerScript.rigidBody.velocity.x != 0f)
            animator.SetBool("isMovingHorizontally", true);
        else
            animator.SetBool("isMovingHorizontally", false);

        if (playerScript.rigidBody.velocity.y != 0f)
            animator.SetBool("isMovingVertically", true);
        else
            animator.SetBool("isMovingVertically", false);


        if (playerScript.isRolling)
            animator.SetBool("isRolling", true);
        else
            animator.SetBool("isRolling", false);

        if(playerScript.isDashing)
            animator.SetBool("isDashing", true);
        else
            animator.SetBool("isDashing", false);
    }

    void MovementParticles()
    {
        if (playerScript.isGrounded)
        {
            if (playerScript.rigidBody.velocity.x > 0)
                WalkingRightParticles.SetActive(true);
            else
                WalkingRightParticles.SetActive(false);

            if (playerScript.rigidBody.velocity.x < 0)
                WalkingLeftParticles.SetActive(true);
            else
                WalkingLeftParticles.SetActive(false);
        }
        else
        {
            WalkingRightParticles.SetActive(false);
            WalkingLeftParticles.SetActive(false);
        }

        if(playerScript.isDashing || playerScript.isRolling)
        {
            if (playerScript.rigidBody.velocity.x > 0)
                DashingRightParticles.SetActive(true);
            else if (playerScript.rigidBody.velocity.x < 0)
                DashingLeftParticles.SetActive(true);
            
            if (playerScript.rigidBody.velocity.y > 0 && !playerScript.isRolling)
                DashingUpParticles.SetActive(true);
            else if (playerScript.rigidBody.velocity.y < 0 && !playerScript.isRolling)
                DashingDownParticles.SetActive(true);
        }
        else
        {
            DashingRightParticles.SetActive(false);
            DashingLeftParticles.SetActive(false);
            DashingUpParticles.SetActive(false);
            DashingDownParticles.SetActive(false);
        }
    }

    void playerLastPos()
    {
        newPos = transform.position;
        distanceTraveled = newPos.y - oldPos.y;
        oldPos = newPos;
    }

    void SpriteFlipping()
    {
        //Flipping with Velocity
        if (playerScript.rigidBody.velocity.x < 0)
        {
            playerSprite.flipX = true;
        }
        else if (playerScript.rigidBody.velocity.x > 0)
        {
            playerSprite.flipX = false;
        }
    }
}
