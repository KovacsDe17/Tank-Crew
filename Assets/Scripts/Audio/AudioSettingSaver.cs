using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingSaver : MonoBehaviour
{
    [SerializeField] private Slider menuMusicSlider;
    [SerializeField] private Slider ingameMusicSlider;
    [SerializeField] private Slider menuSoundSlider;
    [SerializeField] private Slider ingameSoundSlider;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Debug.Log("A - Initializing AudioSettingSaver");

        menuMusicSlider.onValueChanged.AddListener((value) =>
        {
            SaveAndLoad.SaveAudioSetting(SaveAndLoad.MENU_MUSIC_KEY, value);
        });

        ingameMusicSlider.onValueChanged.AddListener((value) =>
        {
            SaveAndLoad.SaveAudioSetting(SaveAndLoad.INGAME_MUSIC_KEY, value);
        });

        menuSoundSlider.onValueChanged.AddListener((value) =>
        {
            SaveAndLoad.SaveAudioSetting(SaveAndLoad.MENU_SOUND_KEY, value);
        });

        ingameSoundSlider.onValueChanged.AddListener((value) =>
        {
            SaveAndLoad.SaveAudioSetting(SaveAndLoad.INGAME_SOUND_KEY, value);
        });

        UpdateSliderValues();
    }

    public void SaveSettings()
    {
        SaveAndLoad.ForceSave();
    }

    private void OnLevelWasLoaded(int level)
    {
        //Initialize();
    }

    private void UpdateSliderValues()
    {
        Debug.Log("A - Updating slider values in AudioSettingSaver");
        menuMusicSlider.value = SaveAndLoad.LoadAudioSetting(SaveAndLoad.MENU_MUSIC_KEY);
        ingameMusicSlider.value = SaveAndLoad.LoadAudioSetting(SaveAndLoad.INGAME_MUSIC_KEY);
        menuSoundSlider.value = SaveAndLoad.LoadAudioSetting(SaveAndLoad.MENU_SOUND_KEY);
        ingameSoundSlider.value = SaveAndLoad.LoadAudioSetting(SaveAndLoad.INGAME_SOUND_KEY);
    }
}
