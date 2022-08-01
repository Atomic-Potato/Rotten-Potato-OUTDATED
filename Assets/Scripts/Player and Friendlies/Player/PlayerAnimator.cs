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
    //Variables
    [SerializeField] float flippedAngle;

    [Space]

    //GameObjects
    [SerializeField] GameObject playerBody;
    [SerializeField] GameObject pivot1;

    [Space]

    //Components
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] SpriteRenderer weaponSprite;

    [Space]

    //Scripts
    [SerializeField] PlayerController playerScript;
    [SerializeField] PivotController pivotController;
    
    [Space]

    [SerializeField] GameObject WalkingRightParticles;
    [SerializeField] GameObject WalkingLeftParticles;
    [SerializeField] GameObject DashingRightParticles;
    [SerializeField] GameObject DashingLeftParticles;
    [SerializeField] GameObject DashingUpParticles;
    [SerializeField] GameObject DashingDownParticles;

    [Space]
    
    //Privates
    float oldAngle;

    Vector3 oldPos;
    Vector3 newPos;

    [HideInInspector] public float distanceTraveled;

    void Start()
    {
        oldAngle = pivotController.pivotMax;

        oldPos = transform.position;
    }

    void Update()
    {
        SpriteFlipping(pivot1);
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

    void SpriteFlipping(GameObject pivot)
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

        if (pivotController.rotationAngle < pivotController.pivotMin || pivotController.rotationAngle > pivotController.pivotMax)
        {
            playerSprite.flipX = true;
            pivotController.pivotMax = flippedAngle;
            pivotController.pivotMin = flippedAngle * -1; //When flipped, the agnles dont flip as well, for example, when it flips on 120 it will flip back on 120 wich is not when the pivot is behind the back 
        }
        else if (pivotController.rotationAngle > pivotController.pivotMin || pivotController.rotationAngle < pivotController.pivotMax)
        {
            playerSprite.flipX = false;
            pivotController.pivotMax = oldAngle;
            pivotController.pivotMin = oldAngle * -1;
        }

        //Flipping arm sprite
        if (playerSprite.flipX == false)
            weaponSprite.flipY = false;
        else if (playerSprite.flipX == true)
            weaponSprite.flipY = true;

        //Flipping the pivot angle when moving
        if (playerScript.rigidBody.velocity.x > 0)
        {
            pivotController.pivotMax = oldAngle;
            pivotController.pivotMin = oldAngle * -1;
        }
        else if (playerScript.rigidBody.velocity.x < 0)
        {
            pivotController.pivotMax = flippedAngle;
            pivotController.pivotMin = flippedAngle * -1;
        }
    }
}
