using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedrunController : MonoBehaviour
{
    [SerializeField] GameObject timer;
    [SerializeField] SpeedrunTimer timerScript;
    [SerializeField] MusicController musicController;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player") && this.CompareTag("Start Line"))
        {
            timer.SetActive(true);
            timerScript.currentTime = 0f;
            timerScript.finished = false;

            musicController.playSpeedCourse = true;
        }
        else if(other.CompareTag("Player") && this.CompareTag("Finish Line"))
        {
            timerScript.finished = true;
            musicController.playSpeedCourse = false;
        }
    }
}
