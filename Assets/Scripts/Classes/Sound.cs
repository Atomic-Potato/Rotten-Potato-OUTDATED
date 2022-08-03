using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    //Public 
    public string name;
    public AudioClip clip;    
    [Range(0f, 1f)] 
    public float initialVolume;
    [Range(0f, 500f)]
    public float minDistanceToHear = 3.5f;
    [Range(-3f, 3f)] 
    public float pitch = 1f;
    [Range(0, 10f)] 
    public float FadeInTime;
    [Range(0, 10f)] 
    public float FadeOutTime;
    public bool loop;
    public bool playOnAwake;

    //Hidden
    [HideInInspector] public AudioSource source;

    //Private
    GameObject parent;

    bool fadingIn;
    bool fadingOut;

    public void AssignToObject(GameObject gameObject)
    {
        source = gameObject.AddComponent<AudioSource>();
        source.name = name;
        source.clip = clip;
        source.volume = initialVolume;
        source.minDistance = minDistanceToHear;
        source.pitch = pitch;
        source.loop = loop;
        source.playOnAwake = playOnAwake;

        parent = gameObject;

        source.volume = 0;
    }

    //Getters
    public bool isFadingIn()
    {
        return fadingIn;
    }
    public bool isFadingOut()
    {
        return fadingOut;
    }

    //Setters
    public void FadeIn(bool fadeIn)
    {
        fadingIn = fadeIn;
    }

    public void FadeOut(bool fadeOut)
    {
        fadingOut = fadeOut;
    }
}