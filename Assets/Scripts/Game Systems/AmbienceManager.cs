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
    [SerializeField] int minSoundsBeforeSpecial;
    [SerializeField] int maxSoundsBeforeSpecial;

    [Space]
    [Header("Sounds")]
    [SerializeField] Sound[] loopingSounds;
    [SerializeField] Sound[] randomSounds;
    [SerializeField] Sound[] specialRandomSounds;

    //Privates
    int maxRegulars;
    int regularsPlayed;
    Coroutine playSoundCache = null;

    void Awake()
    {
        AddSoundsToParent(loopingSounds);
        AddSoundsToParent(randomSounds);
        AddSoundsToParent(specialRandomSounds);
    }

    void Start()
    {
        maxRegulars = Random.Range(minSoundsBeforeSpecial, maxSoundsBeforeSpecial);
    }

    void Update()
    {
        if(playSoundCache == null)
        {
            Sound s;
            float timeBeforeNext = Random.Range(minTimeToPlay, maxTimeToPlay);

            if(regularsPlayed <= maxRegulars)
            {
                int toPlayIndex = Random.Range(0, randomSounds.Length);
                s = randomSounds[toPlayIndex];

                regularsPlayed++;
            }
            else
            {
                int toPlayIndex = Random.Range(0, specialRandomSounds.Length);
                s = randomSounds[toPlayIndex];

                regularsPlayed = 0;
                maxRegulars = Random.Range(minSoundsBeforeSpecial, maxSoundsBeforeSpecial);
            }

            PlaySound(s, timeBeforeNext);
        }
    }

    IEnumerator PlaySound(Sound s, float time)
    {
        yield return new WaitForSeconds(time);
        s.source.Play();
        playSoundCache = null;
    }

    void AddSoundsToParent(Sound[] sounds)
    {
        foreach(Sound s in sounds)
        {
            s.AssignToObject(parentObject);

            if(s.playOnAwake)
                s.source.Play();
        }
    }
}
