using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject keybindsMenu;
    [SerializeField] GameObject controlsMenu;
    [SerializeField] GameObject creditsMenu;
    [SerializeField] GameObject quitMenu;
    [SerializeField] string currentSceneName;

    [Space] [Header("Game Modifiers")]
    [SerializeField]  Text dashCountText;

    [Space] [Header("Various objects")]
    [SerializeField] GameObject[] icons;
    [SerializeField] GameObject[] controlsInfos;
    [SerializeField] GameObject timer;
    [SerializeField] PlayerController playerController;

    [HideInInspector] public static bool gameIsPaused = false;

    bool timerState;
    
    void Start()
    {
        dashCountText.text = playerController.dashesCount.ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Pausing game
            if (gameIsPaused)
                Resume();
            else
            {
                //we get if the timer is active or not before we pause
                timerState = timer.activeSelf ? true : false;
                Pause();
            }
        }
    }

    public void Pause()
    {
        gameIsPaused = true;
        TrunOffAllMenus();
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;

        foreach (GameObject icon in icons)
            icon.SetActive(false);
        
        //if the timer was active we deactivated
        if(timerState) 
            timer.SetActive(false);
    }


    public void Resume()
    {
        gameIsPaused = false;
        TrunOffAllMenus();
        Time.timeScale = 1f;

        foreach (GameObject icon in icons)
            icon.SetActive(true);
        
        //if the timer was active before we paused then we activate
        if(timerState)
            timer.SetActive(true);
    }

    void TrunOffAllMenus()
    {
        pauseMenu.SetActive(false);
        keybindsMenu.SetActive(false);
        controlsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        quitMenu.SetActive(false);
    }

    public void Keybinds()
    {
        pauseMenu.SetActive(false);
        keybindsMenu.SetActive(true);
    }

    public void MoreControls()
    {
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void GrapplingInfo()
    {
        foreach (GameObject info in controlsInfos)
            info.SetActive(false);
        controlsInfos[0].SetActive(true);
    }

    public void DashingInfo()
    {
        foreach (GameObject info in controlsInfos)
            info.SetActive(false);
        controlsInfos[2].SetActive(true);
    }

    public void AimingAndShootingInfo()
    {
        foreach (GameObject info in controlsInfos)
            info.SetActive(false);
        controlsInfos[3].SetActive(true);
    }

    public void RollingInfo()
    {
        foreach (GameObject info in controlsInfos)
            info.SetActive(false);
        controlsInfos[1].SetActive(true);
    }

    public void CameraInfo()
    {
       foreach (GameObject info in controlsInfos)
            info.SetActive(false);
        controlsInfos[4].SetActive(true); 
    }


    public void ReloadScene()
    {
        Resume();
        SceneManager.LoadScene(currentSceneName);
    }

    public void Credits()
    {
        pauseMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }

    public void QuitButton()
    {
        pauseMenu.SetActive(false);
        quitMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void IncreaseDashCount()
    {
        playerController.dashesCount++;
        dashCountText.text = playerController.dashesCount.ToString();
    }
    public void DecreaseDashCount()
    {
        if(playerController.dashesCount > 0)
        {
            playerController.dashesCount--;
            dashCountText.text = playerController.dashesCount.ToString();
        }
    }
}
