using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button resumeBtn;
    [SerializeField] private Button mainMenuBtn;

    [Header("Settings")] [SerializeField] private Button music, sfx;
    [SerializeField] private Image musicIcon, sfxIcon;
    [SerializeField] private Sprite musicOn, musicOff, sfxOn, sfxOff;

    [Header("SFX")] [SerializeField] AudioClip buttonClickSFX;
    private void Start()
    {
        GameManager.Instance.OnGamePause += GameManager_OnGamePause;
        
        resumeBtn.onClick.AddListener(() => { GameManager.Instance.TogglePause(); });
        mainMenuBtn.onClick.AddListener(() => { UnityEngine.SceneManagement.SceneManager.LoadScene(0); });

        UpdateSettingsVisuals();
        music.onClick.AddListener(() =>
        {
            AudioManager.Instance.ToggleMusic();
            UpdateSettingsVisuals();
        });
        sfx.onClick.AddListener(() =>
        {
            AudioManager.Instance.ToggleSFX();
            UpdateSettingsVisuals();
        });
        AddButtonSounds();
        Hide();
    }
    
    private void OnDestroy()
    {
        GameManager.Instance.OnGamePause -= GameManager_OnGamePause;
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
    }

    public void Hide()
    {
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
    
    void AddButtonSounds()
    {
        //find all uttons in scene and add sound on click
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => { AudioManager.Instance.PlaySound(buttonClickSFX); });
        }
    }
}