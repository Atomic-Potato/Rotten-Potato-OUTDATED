using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] AudioManager audioManagerScript;
    [SerializeField] PlayerController playerController;

    string previousState = null;    

    Coroutine fadeInCache;
    Coroutine fadeOutCache;

    void Update()
    {
        
        string state = GetState();
        
        //if(previousState != state)
        //    Debug.Log(state);

        if(previousState == null || state != previousState)
        {
            //Fade caches stop coroutines from executing multiple times
            fadeInCache = null;
            fadeOutCache = null;
        }

        if(fadeOutCache == null && previousState != null && state != null && previousState != state)
            fadeOutCache = StartCoroutine(AudioManager.FadeOut(previousState, audioManagerScript.player));
        if(fadeInCache == null)
            fadeInCache = StartCoroutine(AudioManager.FadeIn(state, audioManagerScript.player));

        previousState = state;
    }

    public string GetState()
    {
        if(playerController.grapplingLoaded)
            return "Grappling Ready";
            
        if(playerController.isJustGrappling)
            return "On Grappling";

        if(playerController.isJustBrokeGrappling)
            return "On Grappling Break";

        if(playerController.isJustFinishedGrappling)
            return "On Grappling Finish";

        if(playerController.isGrappling)
            return "Grappling";

        if(playerController.isJustDashing)
            return "On Dashing";
        
        if(playerController.isDashing)
            return "Dashing";

        if(playerController.isJumping)
            return "Jump";

        if(playerController.isJustLanded)
            return "On Grounded";

        if(playerController.isRolling)
            return "Rolling";

        if(playerController.isMoving)
            return "Grounded Movement";

        return "Idle";
    }
}
