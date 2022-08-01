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
            Debug.Log("Started Playing");
            s.source.Play();
            s.source.volume = 1;
        }
        else
            Debug.Log("Already Playing");

        return 1;
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


    public static IEnumerator FadeOut(string name, Sound[] entity)
    {
        Sound s = Array.Find(entity, sound => sound.name == name);
        
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

        if(!s.source.isPlaying)
            yield break;

        float rate = s.maxVolume/s.FadeOutTime; //The equation for acceleration

        while(s.volume > 0)
        {
            s.source.volume -= rate * Time.deltaTime;
            yield return new WaitForSeconds(s.FadeOutTime);
            s.source.Stop();
        }
    }

    public static IEnumerator FadeIn(string name, Sound[] entity)
    {
        Sound s = Array.Find(entity, sound => sound.name == name);
        
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

        if(s.source.isPlaying)
            yield break;

        s.source.Play();

        float rate = s.maxVolume / s.FadeInTime; //The equation for acceleration

        while(s.volume < s.maxVolume)
        {
            Debug.Log(s.source.volume);
            s.source.volume += rate * Time.deltaTime;
            yield return new WaitForSeconds(s.FadeInTime);
        }

    }
}
