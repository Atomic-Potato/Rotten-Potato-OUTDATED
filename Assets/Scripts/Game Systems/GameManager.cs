using System;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    float _timeScaleBeforePause;
    
    public Action PauseGame;
    public Action ResumeGame;

    static GameManager _instance;
    public static GameManager Instance => _instance;


    GameState _currentGameState;
    public GameState CurrentGameState => _currentGameState;
    public enum GameState
    {
        Playing,
        Paused
    }

    void Awake() 
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
        
        _currentGameState = GameState.Playing;
        
        PauseGame = ExecutePauseGame;
        ResumeGame = ExecuteResumeGame;

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (PlayerInputManager.Maps.UI.Pause.triggered)
        {
            if (_currentGameState != GameState.Paused)
                PauseGame.Invoke();
            else
                ResumeGame.Invoke();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ExecutePauseGame()
    {
        _currentGameState = GameState.Paused;
        _timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f;
        PlayerInputManager.Maps.Player.Disable();
    }

    public void ExecuteResumeGame()
    {
        _currentGameState = GameState.Playing;
        Time.timeScale = _timeScaleBeforePause;
        PlayerInputManager.Maps.Player.Enable();
    }
}