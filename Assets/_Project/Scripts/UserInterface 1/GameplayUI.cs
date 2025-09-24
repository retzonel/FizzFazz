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
        timerText.text = GameTimer.Instance.GetTimeFormatted();
    }
}