using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveAndLoad
{
    public const string MENU_MUSIC_KEY = "menuMusicVolume";
    public const string INGAME_MUSIC_KEY = "ingameMusicVolume";
    public const string MENU_SOUND_KEY = "menuSoundVolume";
    public const string INGAME_SOUND_KEY = "ingameSoundVolume";

    public static void SaveAudioSettings(float menuMusicVolume, float ingameMusicVolume, float menuSoundVolume, float ingameSoundVolume)
    {
        PlayerPrefs.SetFloat(MENU_MUSIC_KEY, menuMusicVolume);
        PlayerPrefs.SetFloat(INGAME_MUSIC_KEY, ingameMusicVolume);
        PlayerPrefs.SetFloat(MENU_SOUND_KEY, menuSoundVolume);
        PlayerPrefs.SetFloat(INGAME_SOUND_KEY, ingameSoundVolume);

        PlayerPrefs.Save();

        AudioManager.Instance.UpdateVolumeSettings();
    }

    public static void SaveAudioSetting(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
        AudioManager.Instance.UpdateVolumeSettings();
    }

    public static void ForceSave()
    {
        PlayerPrefs.Save();
    }

    public static float LoadAudioSetting(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            Debug.LogWarning("Key " + key + " has not been found in PlayerPrefs! Assigning 1f by default!");
        }

        return PlayerPrefs.GetFloat(key, 1f);
    }
}
