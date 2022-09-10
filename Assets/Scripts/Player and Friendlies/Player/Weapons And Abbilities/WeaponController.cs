using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    #pragma warning disable 0649
    
    #region Public Variables
    [Header("Weapon")]
    [SerializeField] float fireRate;
    [SerializeField] float damage;
    [Space]
    [Header("Other Components")]
    [SerializeField] LayerMask Ignore;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform firePoint;
    [SerializeField] PivotController pivotManager;
    [SerializeField] AudioManager audioManager;
    #endregion

    #region Public And Hidden Variables 
    [HideInInspector] public bool isJustShot;
    #endregion

    #region Private Variables
    float timeToFire;
    
    // ---------- Input ----------
    bool fireInputReceived;
    #endregion

    void Update()
    {
        CheckWeapon();
    }

    void CheckWeapon()
    {
        if (fireRate == 0) // For single fire
        {
            if (fireInputReceived)
                Shoot();
        }
        else //For rapid fire
        {
            if (fireInputReceived && Time.time > timeToFire) //Time.time is the current time
            {
                /*basically when you hold the fire button, it shoots once, 
                and then create this variable which was the time the bullet
                got shot plus the delay then it waits this too shoot again and so on*/

                timeToFire = Time.time + 1 / fireRate;
                Shoot();
                StartCoroutine(AudioManager.FadeIn("Shooting", audioManager.player));
            }
        }
    }

    void Shoot()
    {
        if (!pivotManager.locked)
        {
            Instantiate(bullet, firePoint.position, firePoint.rotation);
            StartCoroutine(EnableThenDisable(_ => isJustShot = _, 0.01f));
        }
    }

    public void FireInput(InputAction.CallbackContext context)
    {
        fireInputReceived = context.performed;
    }

    IEnumerator EnableThenDisable(Action<bool> switcher, float time)
    {
        switcher(true); // true => global = true;
        yield return new WaitForSeconds(time);
        switcher(false); // false => global = false; 
    }
}
