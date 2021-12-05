using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuButtons : Singleton<OptionsMenuButtons>
{
    [SerializeField]
    private Slider musicSlider, SFXSlider;
    [SerializeField]
    private Button backButton;

    private void OnEnable()
    {
        backButton.onClick.AddListener(() => UIManager.Instance.OnButtonClicked(UIManager.MyButton.back));
        musicSlider.onValueChanged.AddListener(SoundManager.Instance.SetMusicVolume);
        SFXSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
    }

    private void Start()
    {
        SetSliderValues();
    }

    public void SetSliderValues()
    {
        musicSlider.value = SoundManager.Instance.musicVolume * musicSlider.maxValue;
        SFXSlider.value = SoundManager.Instance.SFXVolume * musicSlider.maxValue;
    }

    private void OnDisable()
    {
        backButton.onClick.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetMusicVolume);
        SFXSlider.onValueChanged.RemoveListener(SoundManager.Instance.SetSFXVolume);
    }
}
