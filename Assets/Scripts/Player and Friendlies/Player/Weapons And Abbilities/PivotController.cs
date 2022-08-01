using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotController : MonoBehaviour
{
#pragma warning disable 0649
    //Publics
    [SerializeField] float pivotSpeed; //Must be really high for it to stay pointing at the mouse 
    [SerializeField] float resetSpeed = 10f;
    public float pivotMin;
    public float pivotMax;

    [SerializeField] PlayerController playerScript;
    [SerializeField] CompanionController compCont;

    //Weapons
    [SerializeField] GameObject weapon1;
    //Pivots
    [SerializeField] GameObject pivot1;

    //Hidden
    [HideInInspector] public bool locked = false; //It tells when the player weapon pivot is locked to tell the companion or the player if to shoot or not when clicking

    [HideInInspector] public float mouseAngle;
    [HideInInspector] public float rotationAngle;

    [HideInInspector] public GameObject functionPivot;

    //Privates
    float speed;


    void FixedUpdate()
    {
        RotateWeapon(pivot1);
    }

    void RotateWeapon(GameObject pivot) //We also handle the rotation of the companion from here, save some time
    {
        functionPivot = pivot; //For using the function's pivot outside of the function

        /*instead of giving the rotation quaternion the mouse angle directly, 
        we use a second float to reset the rotation angle if conditions are met
        otherwise we use the mouseAngle directly*/

        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - pivot.transform.position;
        mouseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (playerScript.rigidBody.velocity.x < 0)
        {
            if (mouseAngle < pivotMax && mouseAngle > pivotMin)
            {
                speed = resetSpeed;

                rotationAngle = 180f;
                compCont.Rotation(compCont.mouseAngle);

                compCont.companionSprite.flipY = false;
                locked = true;
            }
            else if (mouseAngle > pivotMin && mouseAngle < pivotMax)
            {
                speed = resetSpeed;

                rotationAngle = -180f;
                compCont.Rotation(compCont.mouseAngle);

                compCont.companionSprite.flipY = false;
                locked = true;
            }
            else
            {
                speed = pivotSpeed;

                rotationAngle = mouseAngle;
                compCont.Rotation(180f);

                compCont.companionSprite.flipY = true;
                locked = false;
            }
        }
        else if (playerScript.rigidBody.velocity.x > 0)
        {
            if (mouseAngle > pivotMax || mouseAngle < pivotMin)
            {
                speed = resetSpeed;

                rotationAngle = 0f;
                compCont.Rotation(compCont.mouseAngle);

                compCont.companionSprite.flipY = true;
                locked = true;
            }
            else
            {
                speed = resetSpeed;

                rotationAngle = mouseAngle;
                compCont.Rotation(0f);

                compCont.companionSprite.flipY = false;
                locked = false;
            }
        }
        else if (Mathf.Approximately(0f , playerScript.rigidBody.velocity.x))
        {
            speed = pivotSpeed;
            rotationAngle = mouseAngle;
            locked = false;
        }

        // i didnt put these 2 lines up above because we decide which angle we want to use in the if statements
        Quaternion rotation = Quaternion.AngleAxis(rotationAngle, Vector3.forward);
        pivot.transform.rotation = Quaternion.Lerp(pivot.transform.rotation, rotation, speed * Time.deltaTime);
    }

}
