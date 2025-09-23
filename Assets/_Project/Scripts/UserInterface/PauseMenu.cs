using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu: MonoBehaviour
{
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button mainMenuBtn;
    
    [Header("Settings")] 
    [SerializeField] private Button music, sfx;
    [SerializeField] private Image musicIcon, sfxIcon;
    [SerializeField] private Sprite musicOn, musicOff, sfxOn, sfxOff;
    
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup; // add this to panel

    private void Start()
    {
        resumeBtn.onClick.AddListener(() => { GameManager.Instance.TogglePause(); });
        mainMenuBtn.onClick.AddListener(() => { UnityEngine.SceneManagement.SceneManager.LoadScene(0); });
        
        UpdateSettingsVisuals();
        music.onClick.AddListener(() => {
            AudioManager.Instance.ToggleMusic();
            UpdateSettingsVisuals();
        });
        sfx.onClick.AddListener(() => {
            AudioManager.Instance.ToggleSFX();
            UpdateSettingsVisuals();
        });
        
        GameManager.Instance.OnGamePause += GameManager_OnGamePause;
        HideImmediate();
    }

    private void GameManager_OnGamePause(object sender, EventArgs e)
    {   
        if (GameManager.Instance.IsGamePlaying())
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    
    public void Show()
    {
        gameObject.SetActive(true);

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1f, 0.3f); // fade in
    }

    public void Hide()
    {
        canvasGroup.DOFade(0f, 0.2f)
            .OnComplete(() => gameObject.SetActive(false)); // fade out
    }
    
    private void HideImmediate()
    {
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
    
    
    void UpdateSettingsVisuals()
    {
        if (AudioManager.Instance.IsMusicOn)
            musicIcon.sprite = musicOn;
        else
            musicIcon.sprite = musicOff;

        if (AudioManager.Instance.IsSFXOn)
            sfxIcon.sprite = sfxOn;
        else
            sfxIcon.sprite = sfxOff;
    }
}