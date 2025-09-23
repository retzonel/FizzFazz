using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button pauseButton;

    private void Start()
    {
        pauseButton.onClick.AddListener(() =>
        {
            GameManager.Instance.TogglePause();
        });
        UpdateTimerText();
    }

    private void Update()
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            UpdateTimerText();
        }
    }

    private void UpdateTimerText()
    {
        float timeRemaining = Mathf.Max(0, GameTimer.Instance.GetTimeFormmated());
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}