using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    //Public 
    public string name;
    public AudioClip clip;    
    [Range(0f, 1f)] 
    public float volume;
    [Range(-3f, 3f)] 
    public float pitch = 1f;
    [Range(0, 10f)] 
    public float FadeInTime;
    [Range(0, 10f)] 
    public float FadeOutTime;
    public bool loop;
    public bool playOnAwake;

    //Hidden
    [HideInInspector] public float maxVolume;
    [HideInInspector] public AudioSource source;

    GameObject parent;

    public void AssignToObject(GameObject gameObject)
    {
        source = gameObject.AddComponent<AudioSource>();
        source.name = name;
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
        source.playOnAwake = playOnAwake;

        parent = gameObject;
        maxVolume = volume;
    }
}