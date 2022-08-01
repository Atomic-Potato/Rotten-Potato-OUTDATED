// NOTES:
// Companion rotation when shooting is handelled in the PivotController script

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionController : MonoBehaviour
{
#pragma warning disable 0649
    //Publics
    [SerializeField] float switchSpeedIdle;
    [SerializeField] float switchSpeedHorizontal;
    [SerializeField] float switchSpeedVertical;
    [SerializeField] float rotationSpeed;
    [SerializeField] float idleOffset;
    [SerializeField] float moveOffest;
    [SerializeField] float midAirOffset;

    [Space]
    [SerializeField] SpriteRenderer playerSprite;
    public SpriteRenderer companionSprite;

    [SerializeField] PlayerController playerScript;
    [SerializeField] PlayerAnimator playerAnimSp;

    //Privates
    [HideInInspector] public float mouseAngle;

    Vector3 localPos;
    Vector3 defaultPos;

    float rightOffIdle;//locked
    float leftOffIdle;
    float rightOffset;
    float leftOffset;
    float upOffset;
    float downOffset;

    private void Start()
    {
        localPos = transform.parent.InverseTransformPoint(transform.position);
        defaultPos.y = localPos.y;

        rightOffset = localPos.x + moveOffest;
        leftOffset = localPos.x - moveOffest;

        upOffset = localPos.y - midAirOffset;
        downOffset = (localPos.y + midAirOffset)/2;

        rightOffIdle = localPos.x + idleOffset;
        leftOffIdle = localPos.x - idleOffset;
    }

    void FixedUpdate() // ty colorfurrrrr for reminding me to use fixed instead uwu
    {
        Movement();
    }

    void Movement() //Movement handles which pivot to be on with respect to the player and its rotation with respect to the weapon's rotation
    {
        //We flip the way depending on each case to keep the companion top always facing the sky
        //We used new Vector3 because the x and y are interfering eachother if we use Vector3.Lerp

        //movement on horizontal axis
        if (Mathf.Approximately(0f, playerScript.rigidBody.velocity.x))//when idle
        {
            if (playerSprite.flipX == false)
            {
                transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, leftOffIdle, switchSpeedIdle * Time.deltaTime), transform.localPosition.y, transform.localPosition.z);
                Rotation(0f);
                companionSprite.flipY = false;
            }
            else
            {
                transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, rightOffIdle, switchSpeedIdle * Time.deltaTime), transform.localPosition.y, transform.localPosition.z);
                Rotation(180f);
                companionSprite.flipY = true;
            }
        }
        else if (playerScript.rigidBody.velocity.x < 0f)     //when going left
            transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, rightOffset, switchSpeedHorizontal * Time.deltaTime), transform.localPosition.y, transform.localPosition.z);
        else if (playerScript.rigidBody.velocity.x > 0f)      //when going right
            transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, leftOffset, switchSpeedHorizontal * Time.deltaTime), transform.localPosition.y, transform.localPosition.z);

        //movement on the vertical axis
        if (Mathf.Approximately(0f,playerScript.rigidBody.velocity.y))
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultPos.y, switchSpeedVertical * Time.deltaTime), transform.localPosition.z);
        else if (playerScript.rigidBody.velocity.y > 0)
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, upOffset, switchSpeedVertical * Time.deltaTime), transform.localPosition.z);
        else if (playerScript.rigidBody.velocity.y < 0)
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, downOffset, switchSpeedVertical * Time.deltaTime), transform.localPosition.z);
    }

    public void Rotation(float angleRotation) //Handles rotation
    {
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position; //I tried using the weapon's rotation but its not actually the same angle between the comp and the mouse, thats because they are not on the same Y• 
        mouseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angleRotation, Vector3.forward);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

}
