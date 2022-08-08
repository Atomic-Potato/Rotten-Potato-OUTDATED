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
    public bool playOnAwake;

    [Tooltip("If loop is disabled then fade in and out are ineffective")]
    public bool loop;
    [Range(0, 10f)] public float FadeInTime;
    [Range(0, 10f)] public float FadeOutTime;

    //Hidden
    [HideInInspector] public GameObject parent;
    [HideInInspector] public AudioSource source;
    
    //Private
    bool fadingIn;
    bool fadingOut;

    public Sound(Sound s)
    {
        parent = s.parent;

        source = parent.AddComponent<AudioSource>();
        source.name = s.name;
        source.clip = s.clip;
        source.volume = s.initialVolume;
        source.minDistance = s.minDistanceToHear;
        source.pitch = s.pitch;
        source.loop = s.loop;
        source.playOnAwake = s.playOnAwake;

        source.volume = 0;
    }

    public void AssignToObject(GameObject gameObject)
    {
        parent = gameObject;

        source = parent.AddComponent<AudioSource>();
        source.name = name;
        source.clip = clip;
        source.volume = initialVolume;
        source.minDistance = minDistanceToHear;
        source.pitch = pitch;
        source.loop = loop;
        source.playOnAwake = playOnAwake;

        source.volume = initialVolume;
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