using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] float maxTimeToPlay;
    [SerializeField] float minTimeToPlay;
    [SerializeField] int specialsCount;
    [SerializeField] float timeToPlaySpecials;
    [SerializeField] float fadeOutSpeed;

    [SerializeField] AudioSource speedCourseMusic;
    [SerializeField] AudioSource[] waitingForExistanceOverworld; //remember to put specials at the end


    [HideInInspector] public bool playSpeedCourse;
    [HideInInspector] public float originalVolume = 0f;
    [HideInInspector] public AudioSource currentMusicClip;

    int clipNumber;
    int lastClipNumber = -1;
    float timer;
    float specialTimer;
    

    void Start()
    {
        specialTimer = timeToPlaySpecials;
    }

    void Update() 
    {
        //Debug.Log(currentMusicClip);

        if(playSpeedCourse)
        {
            if(!currentMusicClip.isPlaying)
            {
                currentMusicClip = speedCourseMusic;
                currentMusicClip.time = 0f;
                currentMusicClip.Play();
            }
            else if(!speedCourseMusic.isPlaying)
            {
                if(originalVolume == 0f)
                    originalVolume = currentMusicClip.volume;

                FadeOut(fadeOutSpeed, originalVolume);
            }
        }
        else
        {
            if(!speedCourseMusic.isPlaying)
                PlayRandomly(waitingForExistanceOverworld);
            else
            {
                if(originalVolume == 0f)
                    originalVolume = currentMusicClip.volume;

                FadeOut(fadeOutSpeed, originalVolume);
            }
        }     
    }

    void PlayRandomly(AudioSource[] song)
    {
        if(timer <= 0f && specialTimer >= 0)
        {
            timer = Random.Range(minTimeToPlay, maxTimeToPlay);
            do{
                clipNumber = Random.Range(0, song.Length - specialsCount);
            }while(clipNumber == lastClipNumber);
            lastClipNumber = clipNumber;
            
            currentMusicClip = song[clipNumber];
            currentMusicClip.time = 0f;
            currentMusicClip.Play();
        }
        else if (specialTimer <= 0)
        {
            specialTimer = timeToPlaySpecials;

            do{
                clipNumber = Random.Range(song.Length - specialsCount, song.Length);
            }while(clipNumber == lastClipNumber);

            currentMusicClip = song[clipNumber];
            currentMusicClip.time = 0f;
            currentMusicClip.Play();
        }
        else
        {
            timer -= Time.deltaTime;
            specialTimer -= Time.deltaTime;
        }
    }

    public void FadeOut(float speed, float maxVolume)
    {
        if(currentMusicClip.volume > 0)
            currentMusicClip.volume -= speed;
        else
        {
            currentMusicClip.Stop();
            currentMusicClip.volume = maxVolume;
            originalVolume = 0f; // for the next fade out clip
        }
    }
}
