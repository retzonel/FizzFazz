using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playBtn;
    [SerializeField] private Button exitBtn;

    [Header("Settings")] 
    [SerializeField] private Button music, sfx;
    [SerializeField] private Image musicIcon, sfxIcon;
    [SerializeField] private Sprite musicOn, musicOff, sfxOn, sfxOff;
    
    [Header("SFX")] [SerializeField] AudioClip buttonClickSFX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playBtn.onClick.AddListener(() => { UnityEngine.SceneManagement.SceneManager.LoadScene(1); });
        exitBtn.onClick.AddListener(() => { Application.Quit(); });
        UpdateSettingsVisuals();
        music.onClick.AddListener(() => {
            AudioManager.Instance.ToggleMusic();
            UpdateSettingsVisuals();
        });
        sfx.onClick.AddListener(() => {
            AudioManager.Instance.ToggleSFX();
            UpdateSettingsVisuals();
        });
        AddButtonSounds();
        
        GameManager.Instance?.Reset();
    }

    // Update is called once per frame
    void Update()
    {
        
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