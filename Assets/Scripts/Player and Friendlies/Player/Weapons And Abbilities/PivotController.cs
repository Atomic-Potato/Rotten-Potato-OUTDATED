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
        RotateWeapons(pivot1);
    }

    void RotateWeapons(GameObject pivot) // Weapons with an S because the companion is sort of a weapon....
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
                SetRotationValues(resetSpeed, 180f, compCont.mouseAngle, false, true);
            else if (mouseAngle > pivotMin && mouseAngle < pivotMax)
                SetRotationValues(resetSpeed, -180f, compCont.mouseAngle, false, true);
            else
                SetRotationValues(pivotSpeed, mouseAngle, 180f, true, false);
        }
        else if (playerScript.rigidBody.velocity.x > 0)
        {
            if (mouseAngle > pivotMax || mouseAngle < pivotMin)
                SetRotationValues(resetSpeed, 0f, compCont.mouseAngle, true, true);
            else
                SetRotationValues(resetSpeed, mouseAngle, 0f, false, false);
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

    private void SetRotationValues(float speed, float rotationAngle, float companionRotation, bool flipCompanionY, bool locked)
    {
        this.speed = resetSpeed;
        this.rotationAngle = rotationAngle;
        compCont.Rotation(companionRotation);
        compCont.companionSprite.flipY = flipCompanionY;
        this.locked = locked;
    }


}
