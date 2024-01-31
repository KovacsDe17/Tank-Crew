using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is responsible for saving the audio settings.
/// </summary>
public class AudioSettingSaver : MonoBehaviour
{
    public const string MENU_MUSIC_KEY = "menuMusicVolume";
    public const string INGAME_MUSIC_KEY = "ingameMusicVolume";
    public const string MENU_SOUND_KEY = "menuSoundVolume";
    public const string INGAME_SOUND_KEY = "ingameSoundVolume";

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
        menuMusicSlider.onValueChanged.AddListener((value) =>
        {
            SaveAudioSetting(MENU_MUSIC_KEY, value);
        });

        ingameMusicSlider.onValueChanged.AddListener((value) =>
        {
            SaveAudioSetting(INGAME_MUSIC_KEY, value);
        });

        menuSoundSlider.onValueChanged.AddListener((value) =>
        {
            SaveAudioSetting(MENU_SOUND_KEY, value);
        });

        ingameSoundSlider.onValueChanged.AddListener((value) =>
        {
            SaveAudioSetting(INGAME_SOUND_KEY, value);
        });

        UpdateSliderValues();
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }

    private void UpdateSliderValues()
    {
        menuMusicSlider.value = LoadAudioSetting(MENU_MUSIC_KEY);
        ingameMusicSlider.value = LoadAudioSetting(INGAME_MUSIC_KEY);
        menuSoundSlider.value = LoadAudioSetting(MENU_SOUND_KEY);
        ingameSoundSlider.value = LoadAudioSetting(INGAME_SOUND_KEY);
    }
    public static void SaveAudioSettings(
        float menuMusicVolume, 
        float ingameMusicVolume, 
        float menuSoundVolume, 
        float ingameSoundVolume)
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
    public static float LoadAudioSetting(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            Debug.LogWarning("Key " + key + " has not been found in PlayerPrefs! Returning 1f by default!");
        }

        return PlayerPrefs.GetFloat(key, 1f);
    }
}
