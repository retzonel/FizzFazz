using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState CurrentState { get; private set; }
    public GameOverType CurrentGameOverType { get; private set; }
    private bool isGamePaused;

    public event EventHandler OnStateChanged;
    public event EventHandler OnGamePause;
    

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one instance of GameManager found!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Reset();
    }

    public void TogglePause()
    {
        isGamePaused = !isGamePaused;
        if (isGamePaused)
        {
            Time.timeScale = 0f; // Pause the game
        }
        else
        {
            Time.timeScale = 1f; // Resume the game
        }

        OnGamePause?.Invoke(this, EventArgs.Empty);
    }
    
    public void GameOver(GameOverType gameOverType)
    {
        CurrentState = GameState.GameOver;
        CurrentGameOverType = gameOverType;
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsGamePlaying()
    {
        return CurrentState == GameState.Playing && !isGamePaused;
    }

    public bool IsGameOver()
    {
        return CurrentState == GameState.GameOver;
    }

    public bool IsGamePaused()
    {
        return isGamePaused;
    }
    
    public GameOverType GetGameOverType()
    {
        return CurrentGameOverType;
    }
    
    public void Reset()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        CurrentGameOverType = GameOverType.None;
        CurrentState = GameState.Playing;
    }
}

public enum GameState
{
    Playing,
    GameOver
}

public enum GameOverType
{
    None,
    Win, //complete all puzzle in time
    Lose // time run out
}