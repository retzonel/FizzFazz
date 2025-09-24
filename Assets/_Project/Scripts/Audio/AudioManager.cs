using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    public bool IsMusicOn { get; private set; } = true;
    public bool IsSFXOn { get; private set; } = true;

    [Header("MUSIC CLIPS")]
    public AudioClip[] musicClips;
    
    void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("More than one instance of Auido Manager found!");
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    private void Start()
    {
        IsMusicOn = true;
        IsSFXOn = true;
        musicSource.mute = !IsMusicOn;
        musicSource.loop = true;
        sfxSource.mute = !IsSFXOn;
        
        PlayMusic();
    }
    
    void PlayMusic()
    {
        //pick a random music clip and play
        if (musicClips.Length == 0) return;
        int index = UnityEngine.Random.Range(0, musicClips.Length);
        musicSource.clip = musicClips[index];
        musicSource.Play();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void ToggleMusic()
    {
        IsMusicOn = !IsMusicOn;
        musicSource.mute = !IsMusicOn;
    }

    public void ToggleSFX()
    {
        IsSFXOn = !IsSFXOn;
        sfxSource.mute = !IsSFXOn;
    }
}