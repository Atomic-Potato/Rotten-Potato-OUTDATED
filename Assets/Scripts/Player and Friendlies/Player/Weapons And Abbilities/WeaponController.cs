using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
#pragma warning disable 0649
    //Publics
    [Header("Weapon")]
    [SerializeField] float fireRate;
    [SerializeField] float damage;
    [Space]
    [Header("Other Components")]
    [SerializeField] LayerMask Ignore;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform firePoint;
    [SerializeField] PivotController pivotManager;


    //Privates
    float timeToFire;


    void Update()
    {
        CheckWeapon();
    }

    void CheckWeapon()
    {
        if (fireRate == 0) // For single fire
        {
            if (Input.GetButtonDown("Fire1"))
                Shoot();
        }
        else //For rapid fire
        {
            if (Input.GetButton("Fire1") && Time.time > timeToFire) //Time.time is the current time
            {
                /*basically when you hold the fire button, it shoots once, 
                and then create this variable which was the time the bullet
                got shot plus the delay then it waits this too shoot again and so on*/

                timeToFire = Time.time + 1 / fireRate;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        if (!pivotManager.locked)
            Instantiate(bullet, firePoint.position, firePoint.rotation);
    }
}
