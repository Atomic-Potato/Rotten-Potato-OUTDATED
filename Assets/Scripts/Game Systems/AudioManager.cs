using System.Collections;
using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    [SerializeField] GameObject playerAudioObject;
    public Sound[] player;

    public static AudioManager instance; //the keyword static means that this variable is the same between all the instances of the class AudioManager
    
    void Awake()
    {
        //Destroying audio clips holders that we already have an instance of
        if(instance == null)
            instance = this;
        else
        {
            Destroy(gameObject); 
        }

        //Deciding which audio clips holders that should not be destroyed between scenes
        DontDestroyOnLoad(gameObject); //The audio manager

        //Adding the audio sources
        foreach(Sound s in player)
        {
            s.AssignToObject(playerAudioObject);
            s.source.volume = 0;
        }
    }

    public static int PlayClip(string name, Sound[] entity)
    {
        Sound s = Array.Find(entity, sound => sound.name == name);
        
        if(s == null)
        {
            Debug.Log("Clip " + name + " not found in AudioManager.PlayClip()");
            return 0;
        }

        if(entity == null)
        {
            Debug.Log("Entity cannot be null in AudioManager.PlayClip()");
            return 0;
        }

        
        if(!s.source.isPlaying)
        {
            s.source.Play();
            s.source.volume = 1;
        }

        return 1;
    }

    public static IEnumerator PlayClip(Sound s)
    {
        if(s == null)
        {
            Debug.Log("Clip doesnt exist in AudioManager.PlayClip(Sound s)");
            yield break;
        }

        Sound newSound = new Sound(s);

        newSound.source.volume = s.initialVolume;
        newSound.source.time = 0;
        newSound.source.Play();

        yield return new WaitForSeconds(s.source.clip.length);

        Destroy(newSound.source);
    }

    public static int PauseClip(string name, Sound[] entity)
    {
        Sound s = Array.Find(entity, sound => sound.name == name);

        if(s == null)
        {
            Debug.Log("Clip " + name + " not found in AudioManager.PauseClip()");
            return 0;
        }

        if(entity == null)
        {
            Debug.Log("Entity cannot be null in AudioManager.PlayClip()");
            return 0;
        }

        s.source.time = 0;
        s.source.Stop();

        return 1;
    }

    public static IEnumerator PauseClip(Sound s)
    {
        if(s == null)
        {
            Debug.Log("Clip doesnt exist in AudioManager.PauseClip(Sound s)");
            yield break;
        }

        s.source.volume = s.initialVolume;
        s.source.time = 0;
        s.source.Stop();
    }

    public static IEnumerator FadeOut(string name, Sound[] entity)
    {
        Sound s = Array.Find(entity, sound => sound.name == name);
        
        //CHECKING FOR ERRORS
        if(s == null)
        {
            Debug.Log("Clip " + name + " not found in AudioManager.FadeOut()");
            yield break;
        }

        if(entity == null)
        {
            Debug.Log("Entity cannot be null in AudioManager.FadeOut()");
            yield break;
        }

        //If its a non looping audio
        if(!s.loop)
        {
            //yield return PauseClip(s);
            yield break;
        }  

        //FADING OUT
        s.FadeIn(false);
        s.FadeOut(true);

        //if theres no sound or its not playing we stop
        if(!s.source.isPlaying || s.source.volume == 0)
            yield break;

        float rate = s.initialVolume/s.FadeOutTime; //The equation for acceleration

        //while theres volume and the sound should fade out
        while(s.source.volume > 0 && s.isFadingOut())
        {
            s.source.volume -= rate * Time.deltaTime;
            yield return null;
        }

        //if we finish the loop and the FadeOut is not interrupted
        //we stop, because otherwise it will stop it during the FadeIn most likely
        if(s.isFadingOut())
            s.source.Stop();
    }

    public static IEnumerator FadeIn(string name, Sound[] entity)
    {
        Sound s = Array.Find(entity, sound => sound.name == name);
        
        //ERROR CHECKING
        if(s == null)
        {
            Debug.Log("Clip " + name + " not found in AudioManager.FadeIn()");
            yield break;
        }

        if(entity == null)
        {
            Debug.Log("Entity cannot be null in AudioManager.FadeIn()");
            yield break;
        }
        
        //If its a non looping audio
        if(!s.loop)
        {
            yield return PlayClip(s);
            yield break;
        }
        //FADING IN
        s.FadeIn(true);
        s.FadeOut(false);

        if(!s.loop)
        {
            s.source.time = 0;
            s.source.volume = 0;
            s.source.Play();
        }

        if(s.source.volume == 1)
            yield break;

        if(!s.source.isPlaying)
            s.source.Play();

        float rate = s.initialVolume / s.FadeInTime; //The equation for acceleration
        
        while(s.source.volume < s.initialVolume && s.isFadingIn())
        {
            s.source.volume += rate * Time.deltaTime;
            yield return null;
        }

        //if we finish the loop and the FadeIn is not interrupted
        //we set the volume to the exact
        //because otherwise it will suddenly jump to initial volume mid FadeOut
        if(s.isFadingIn())
            s.source.volume = s.initialVolume;
    }
}
