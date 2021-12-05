using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    public static UnityEngine.Events.UnityAction<bool> muteToggled;

    //Separate audio sources allow multiple audio clips to be played with different volume settings simultanesouly
    [SerializeField] private AudioSource musicAudioSource1, musicAudioSource2, SFXAudioSource;
    [SerializeField] private AudioMixer audioMixer;
    private AudioSource currentAudioSource;
    private float audioMixerMinGain = -80.0f;
    private int gainMultiplier = 40;
    private bool isMuted = false;

    [SerializeField]    //Music
    private AudioClip mainMenuBackgroundMusic, mainBackgroundMusic, gameOverBackgroundMusic;
    [SerializeField]    //SFX
    private AudioClip buttonClick, explosion, bulletFiring, gunsPickedUp, extraLifePickedUp;

    public float musicVolume { get; private set; }
    public float SFXVolume { get; private set; }
    private float InitialMusicVolumeLevel = 8;
    private float InitialSFXVolumeLevel = 10;
    private float fadeDuration = 0.5f;

    private void OnEnable()
    {
        //Event subscribers
        SceneManager.sceneLoaded += ChangeBackgroundMusic;  //When a new scene is loaded change the background music
        UIManager.buttonClicked += PlayButtonClick;
        PlayerController.playerCrashed += PlayExplosion;
        AsteroidController.asteroidDestroyed += PlayExplosion;
        Bullet.bulletFired += PlayBulletFiring;
        GunsPickup.pickedUp += PlayGunsPickedUp;
        ExtraLifePickup.pickedUp += PlayExtraLifePickedUp;
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolumeLevel"))
            InitialMusicVolumeLevel = PlayerPrefs.GetFloat("MusicVolumeLevel");

        if (PlayerPrefs.HasKey("SFXVolumeLevel"))
            InitialSFXVolumeLevel = PlayerPrefs.GetFloat("SFXVolumeLevel");

        SetMusicVolume(InitialMusicVolumeLevel);
        SetSFXVolume(InitialSFXVolumeLevel);

        if (PlayerPrefs.GetInt("IsMuted") == 1)
            ToggleMute();
    }

    private void ChangeBackgroundMusic(Scene newScene, LoadSceneMode unused)
    {
        StopAllCoroutines();
        if (currentAudioSource != null)
        {
            //StartCoroutine(FadeOutMusic(currentAudioSource));
            currentAudioSource.Stop();
        }

        if (currentAudioSource == musicAudioSource1)
            currentAudioSource = musicAudioSource2;
        else
            currentAudioSource = musicAudioSource1;

        //Set the AudioClip to the background music for the new scene
        if (newScene == SceneManager.GetSceneByName("MainMenu"))    //Can't use a switch statment because Scene cannot be declared const
            currentAudioSource.clip = mainMenuBackgroundMusic;
        else if (newScene == SceneManager.GetSceneByName("Main"))
            currentAudioSource.clip = mainBackgroundMusic;
        else if (newScene == SceneManager.GetSceneByName("GameOver"))
            currentAudioSource.clip = gameOverBackgroundMusic;

        StartCoroutine(FadeInMusic());
    }

    private IEnumerator FadeInMusic()
    {
        currentAudioSource.volume = 0;
        currentAudioSource.Play();

        float currentTime = 0;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.unscaledDeltaTime;
            currentAudioSource.volume = Mathf.Lerp(0, 1, currentTime / fadeDuration);
            yield return null;
        }
    }

    private IEnumerator FadeOutMusic(AudioSource audioSource)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(start, 0, currentTime / fadeDuration);
            yield return null;
        }
        audioSource.Stop();
    }

    //Separate methods required to play different SFX so that they can be subscribed to different events
    private void PlayButtonClick()
    {
        SFXAudioSource.PlayOneShot(buttonClick);
    }

    private void PlayExplosion()
    {
        SFXAudioSource.PlayOneShot(explosion);
    }

    private void PlayBulletFiring()
    {
        SFXAudioSource.PlayOneShot(bulletFiring);
    }

    private void PlayGunsPickedUp()
    {
        SFXAudioSource.PlayOneShot(gunsPickedUp);
    }

    private void PlayExtraLifePickedUp()
    {
        SFXAudioSource.PlayOneShot(extraLifePickedUp);
    }

    public void SetMusicVolume(float volumeLevel)
    {
        musicVolume = volumeLevel / 10;   //Normalise music volume
        if (musicVolume == 0)
        {
            audioMixer.SetFloat("MusicVolume", audioMixerMinGain);
            if (SFXVolume == 0 && !isMuted)     //If both music and SFX volume are 0 then mute the game
                ToggleMute();
        }
        else
        {
            audioMixer.SetFloat("MusicVolume", ((musicVolume - 1.0f) * gainMultiplier) - 6.0f); //-6.0 dB halves the amplitude
                                                                                              //regardless of musicVolume
            if (isMuted)        //If the game is muted unmute the game
                ToggleMute();
        }

        PlayerPrefs.SetFloat("MusicVolumeLevel", volumeLevel);
    }

    public void SetSFXVolume(float volumeLevel)
    {
        SFXVolume = volumeLevel / 10;     //Normalise SFX volume
        if (SFXVolume == 0)
        {
            audioMixer.SetFloat("SFXVolume", audioMixerMinGain);
            if (musicVolume == 0 && !isMuted)   //If both music and SFX volume are 0 then mute the game
                ToggleMute();
        }
        else
        {
            audioMixer.SetFloat("SFXVolume", (SFXVolume - 1.0f) * gainMultiplier);
            if (isMuted)        //If the game is muted unmute the game
                ToggleMute();
        }

        PlayerPrefs.SetFloat("SFXVolumeLevel", volumeLevel);
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        muteToggled?.Invoke(isMuted);

        if (isMuted)
            audioMixer.SetFloat("MasterVolume", audioMixerMinGain);
        else
        {
            audioMixer.SetFloat("MasterVolume", 0f);
            if (musicVolume == 0 && SFXVolume == 0)
            {
                SetMusicVolume(InitialMusicVolumeLevel);
                SetSFXVolume(InitialSFXVolumeLevel);

                OptionsMenuButtons.Instance.SetSliderValues();
            }
        }

        PlayerPrefs.SetInt("IsMuted", isMuted == true ? 1 : 0);
    }

    private void OnDisable()
    {
        //Event unsubscribe
        SceneManager.sceneLoaded -= ChangeBackgroundMusic;
        UIManager.buttonClicked -= PlayButtonClick;
        PlayerController.playerCrashed -= PlayExplosion;
        AsteroidController.asteroidDestroyed -= PlayExplosion;
        Bullet.bulletFired -= PlayBulletFiring;
    }
}
