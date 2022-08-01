using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionAnimator : MonoBehaviour
{
    [SerializeField] PlayerController playerSp;
    [SerializeField] Animator compAnimator;

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        if (Mathf.Approximately(0f, playerSp.rigidBody.velocity.x))
            compAnimator.SetBool("Moving", true);
        else
            compAnimator.SetBool("Moving", false);
    }

}
