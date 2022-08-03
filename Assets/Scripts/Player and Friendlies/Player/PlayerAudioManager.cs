using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] AudioManager audioManagerScript;
    [SerializeField] PlayerController playerScript;

    string previousState = null;    

    Coroutine fadeInCache;
    Coroutine fadeOutCache;

    void Update()
    {
        string state = GetState();
        
        Debug.Log(state);

        if(previousState == null || state != previousState)
        {
            //Fade caches stop coroutines from executing multiple times
            fadeInCache = null;
            fadeOutCache = null;
        }

        if(fadeOutCache == null && previousState != null && previousState != state)
            fadeOutCache = StartCoroutine(AudioManager.FadeOut(previousState, audioManagerScript.player));
        if(fadeInCache == null)
            fadeInCache = StartCoroutine(AudioManager.FadeIn(state, audioManagerScript.player));

        previousState = state;
    }

    public string GetState()
    {
        if(playerScript.isMoving)
            return "Grounded Movement";

        return "Idle";
    }
}
