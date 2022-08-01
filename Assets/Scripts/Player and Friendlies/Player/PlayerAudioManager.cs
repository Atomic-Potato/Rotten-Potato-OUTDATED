using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] AudioManager audioManagerScript;
    [SerializeField] PlayerController playerScript;

    string previousState = null;    

    void Update()
    {
        string state = GetState();
        
        Debug.Log(state);

        if(previousState == null || state == previousState)
        {
            previousState = state;
            return;
        }

        //StartCoroutine(AudioManager.FadeOut(previousState, audioManagerScript.player));
        StartCoroutine(AudioManager.FadeIn(state, audioManagerScript.player));

        //AudioManager.PauseClip(previousState, audioManagerScript.player);
        //AudioManager.PlayClip(state, audioManagerScript.player);

        previousState = state;
    }

    public string GetState()
    {
        if(playerScript.isMoving)
            return "Grounded Movement";

        return "Idle";
    }
}
