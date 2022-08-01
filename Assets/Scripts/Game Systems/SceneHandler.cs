using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneHandler : MonoBehaviour
{
    [SerializeField] float transitionTime = 1f;
    [SerializeField] string sceneToLoad;
    [SerializeField] Animator transitionAnimator;
    [SerializeField] MusicController musicController;

    bool switchingScenes;

    void Update()
    {
        if(switchingScenes)
        {
            if(musicController.originalVolume == 0)
                musicController.originalVolume = musicController.currentMusicClip.volume;

            musicController.FadeOut(0.0005f, musicController.originalVolume);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            switchingScenes = true;
            StartCoroutine(LoadScene(sceneToLoad));
        }
    }

    IEnumerator LoadScene(string sceneName)
    {
        transitionAnimator.SetTrigger("Start Transition");

        yield return new WaitForSeconds(transitionTime);
        
        SceneManager.LoadScene(sceneName);

        switchingScenes = false;
    }
}
