using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//This class handles the general ambience in the world, specific ambience sound effects are handled with the AudioManager
//I created a seperate one because each scene will have its own *general* ambience 
public class AmbienceManager : MonoBehaviour
{
    [SerializeField] GameObject parentObject;

    
    [Space]
    [SerializeField] [Range(0f, 120f)] float minTimeToPlay;
    [SerializeField] [Range(0f, 120f)] float maxTimeToPlay;
    [SerializeField] bool playSepicals;
    [SerializeField] int minSoundsBeforeSpecial;
    [SerializeField] int maxSoundsBeforeSpecial;

    [Space]
    [Header("Sounds")]
    [SerializeField] Sound[] loopingSounds;
    [SerializeField] Sound[] randomSounds;
    [SerializeField] Sound[] specialRandomSounds;

    //Privates
    int maxRegulars;
    int regularsPlayed = 0;
    Coroutine playSoundCache = null;

    void Awake()
    {
        AddSoundsToParent(loopingSounds);
        Sound.GiveParent(parentObject, randomSounds);
        Sound.GiveParent(parentObject, specialRandomSounds);
    }

    void Start()
    {
        maxRegulars = Random.Range(minSoundsBeforeSpecial, maxSoundsBeforeSpecial);
    }

    void Update()
    {
        // Random sounds handeling
        if(playSoundCache == null)
        {
            Sound s = null;
            int toPlayIndex;
            float timeBeforeNext = Random.Range(minTimeToPlay, maxTimeToPlay);

            if((!playSepicals || regularsPlayed <= maxRegulars) && randomSounds.Length != 0)
            {
                toPlayIndex = Random.Range(0, randomSounds.Length);
                s = randomSounds[toPlayIndex];

                if(playSepicals)
                    regularsPlayed++;
            }
            else if(specialRandomSounds.Length != 0)
            {
                toPlayIndex = Random.Range(0, specialRandomSounds.Length);
                s = specialRandomSounds[toPlayIndex];

                regularsPlayed = 0;
                maxRegulars = Random.Range(minSoundsBeforeSpecial, maxSoundsBeforeSpecial);
            }

            playSoundCache = StartCoroutine(PlaySound(s, timeBeforeNext));
        }
    }

    IEnumerator PlaySound(Sound s, float time)
    {
        if(s == null)
        {
            playSoundCache = null;
            yield break;
        }

        yield return new WaitForSeconds(time);
        StartCoroutine(AudioManager.PlayClip(s));
        playSoundCache = null;
    }

    

    void AddSoundsToParent(Sound[] sounds)
    {
        foreach(Sound s in sounds)
        {
            s.AssignToObject(parentObject);

            if(s.playOnAwake)
                StartCoroutine(AudioManager.FadeIn(s));
        }
    }
}
