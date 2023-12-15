using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static AudioManager;
public class AudioManager : MonoBehaviour
{
    public enum Sound
    {
        Button_Click,//
        Crank_Turn,//
        Hatch_Close,//
        Landmine_Explosion,
        Load_Ammo_Into_Chamber,//
        Load_New_Ammo,
        Shoot_Button_Click,//Not needed?
        Tank_Exhaust,
        Tank_Hit,//
        Tank_Move_Creak,
        Tank_Shoot//
    }

    public enum Music
    {
        MainTheme,
        GameplayCalm,
        GameplayFight
    }

    public enum Category
    {
        MenuMusic,
        IngameMusic, 
        MenuSound,
        IngameSound,
    }

    public static AudioManager Instance { get; private set; }

    public Dictionary<Sound, float> soundTimerDictionary;
    private Dictionary<Sound, Category> soundCategoryDictionary;
    private Dictionary<Music, Category> musicCategoryDictionary;
    private Dictionary<Category, float> categoryVolumeModifiers;
    private List<AudioHelper> audioHelperList = new List<AudioHelper>();

    public GameObject oneShotGameObject;
    public AudioSource oneShotAudioSource;

    public GameObject musicPlayingGameObject;
    public AudioSource musicPlayingAudioSource;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);

        Initialize();
    }

    public void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();

        soundCategoryDictionary = new Dictionary<Sound, Category>
        {
            { Sound.Button_Click, Category.MenuSound },
            { Sound.Crank_Turn, Category.MenuSound },
            { Sound.Hatch_Close, Category.MenuSound },
            { Sound.Landmine_Explosion, Category.IngameSound },
            { Sound.Load_Ammo_Into_Chamber, Category.MenuSound },
            { Sound.Load_New_Ammo, Category.MenuSound },
            { Sound.Shoot_Button_Click, Category.MenuSound },
            { Sound.Tank_Exhaust, Category.IngameSound },
            { Sound.Tank_Hit, Category.IngameSound },
            { Sound.Tank_Move_Creak, Category.IngameSound },
            { Sound.Tank_Shoot, Category.IngameSound },
        };

        musicCategoryDictionary = new Dictionary<Music, Category>
        {
            { Music.MainTheme, Category.MenuMusic},
            { Music.GameplayCalm, Category.IngameMusic},
            { Music.GameplayFight, Category.IngameMusic}
        };

        categoryVolumeModifiers = new Dictionary<Category, float> 
        {
            { Category.MenuMusic, 0.3f },
            { Category.MenuSound, 0.3f },
            { Category.IngameMusic, 0.3f },
            { Category.IngameSound, 0.3f }
        };

        audioHelperList = new List<AudioHelper>();

        UpdateVolumeSettings();
    }

    public void UpdateVolumeSettings()
    {
        UpdateVolumes(Category.MenuMusic);
        UpdateVolumes(Category.MenuSound);
        UpdateVolumes(Category.IngameMusic);
        UpdateVolumes(Category.IngameSound);
    }

    public void PlaySound(Sound sound, float volume = 1f)
    {
        AudioHelper audioHelper;

        if (CanPlaySound(sound))
        {
            if(oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("Sound");
                DontDestroyOnLoad(oneShotGameObject);
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
                
                audioHelper = oneShotGameObject.AddComponent<AudioHelper>();
            } else
            {
                audioHelper = oneShotGameObject.GetComponent<AudioHelper>();
            }

            Category category = GetCategory(sound);
            float outputVolume = GetVolumeSetting(category) * categoryVolumeModifiers[category];

            audioHelper.audioCategory = category;

            if (!audioHelperList.Contains(audioHelper))
                audioHelperList.Add(audioHelper);

            oneShotAudioSource.volume = outputVolume;
            oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
        }
    }

    public void PlaySound(Sound sound, Vector3 position)
    {
        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;

            Category category = GetCategory(sound);
            float outputVolume = GetVolumeSetting(category) * categoryVolumeModifiers[category];

            AudioHelper audioHelper = soundGameObject.AddComponent<AudioHelper>();
            audioHelper.audioCategory = GetCategory(sound);
            audioHelper.containingList = audioHelperList;
            audioHelperList.Add(audioHelper);

            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudioClip(sound);
            audioSource.maxDistance = 100f;
            audioSource.spatialBlend = 1f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.dopplerLevel = 0f;
            audioSource.volume = outputVolume;
            audioSource.Play();

            Object.Destroy(soundGameObject, audioSource.clip.length);
        }
    }

    public AudioSource AttachConstantSound(Sound sound, Transform transform, float fadeDuration = 0f)
    {
        GameObject soundGameObject = new GameObject("Sound");
        soundGameObject.transform.SetParent(transform);
        soundGameObject.transform.localPosition = Vector3.zero;

        Category category = GetCategory(sound);
        float outputVolume = GetVolumeSetting(category) * categoryVolumeModifiers[category];

        AudioHelper audioHelper = soundGameObject.AddComponent<AudioHelper>();
        audioHelper.audioCategory = GetCategory(sound);
        audioHelper.containingList = audioHelperList;
        audioHelperList.Add(audioHelper);

        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = GetAudioClip(sound);
        audioSource.maxDistance = 100f;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0f;
        audioSource.volume = outputVolume;

        audioSource.loop = true;
        audioSource.Play();

        audioHelper.StartCoroutine(StartFade(audioSource, fadeDuration, outputVolume));

        return audioSource;
    }

    public void PlayMusic(Music music, float fadeDuration = 3f)
    {
        if(musicPlayingGameObject != null && musicPlayingAudioSource != null)
        {
            if (musicPlayingAudioSource.clip == GetAudioClip(music))
                return;

            musicPlayingAudioSource.GetComponent<AudioHelper>().StartCoroutine(
                StartFade(musicPlayingAudioSource, fadeDuration, 0f));
        }

        GameObject musicGameObject = new GameObject("Music");

        Category category = GetCategory(music);
        float outputVolume = GetVolumeSetting(category) * categoryVolumeModifiers[category];

        AudioHelper audioHelper = musicGameObject.AddComponent<AudioHelper>();
        audioHelper.audioCategory = GetCategory(music);
        audioHelper.SetDontDestroyOnLoad(true);
        audioHelper.containingList = audioHelperList;
        audioHelperList.Add(audioHelper);

        AudioSource audioSource = musicGameObject.AddComponent<AudioSource>();
        audioSource.clip = GetAudioClip(music);
        audioSource.spatialBlend = 0f;
        audioSource.dopplerLevel = 0f;
        audioSource.volume = outputVolume;
        audioSource.loop = true;

        audioSource.Play();

        audioHelper.StartCoroutine(StartFade(audioSource, fadeDuration, outputVolume));

        musicPlayingGameObject = musicGameObject;
        musicPlayingAudioSource = audioSource;
    }

    public IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }

        if(targetVolume == 0)
            Object.Destroy(audioSource.gameObject);
    }

    public static AudioClip GetAudioClip(Sound sound)
    {
        foreach(AudioAssets.SoundObject audio in AudioAssets.i.audioArray)
        {
            if(audio.sound == sound)
            {
                return audio.clip;
            }
        }

        Debug.LogError("Audio with name " + sound + " not found!");
        return null;
    }

    public static AudioClip GetAudioClip(Music music)
    {
        foreach (AudioAssets.MusicObject audio in AudioAssets.i.musicArray)
        {
            if (audio.music == music)
            {
                return audio.clip;
            }
        }

        Debug.LogError("Music with name " + music + " not found!");
        return null;
    }

    public Category GetCategory(Sound sound)
    {
        if (!soundCategoryDictionary.ContainsKey(sound))
        {
            Debug.LogError("Category of " + sound + " is not defined! Returning default value!");
            return 0;
        }

        return soundCategoryDictionary[sound];
    }

    public Category GetCategory(Music music)
    {
        if (!musicCategoryDictionary.ContainsKey(music))
        {
            Debug.LogError("Category of " + music + " is not defined! Returning default value!");
            return 0;
        }

        return musicCategoryDictionary[music];
    }

    private static float GetVolumeSetting(Category category)
    {
        switch(category)
        {
            case Category.MenuMusic: return SaveAndLoad.LoadAudioSetting(SaveAndLoad.MENU_MUSIC_KEY);
            case Category.MenuSound: return SaveAndLoad.LoadAudioSetting(SaveAndLoad.MENU_SOUND_KEY);
            case Category.IngameMusic: return SaveAndLoad.LoadAudioSetting(SaveAndLoad.INGAME_MUSIC_KEY);
            case Category.IngameSound: return SaveAndLoad.LoadAudioSetting(SaveAndLoad.INGAME_SOUND_KEY);
            default: return 1f;
        }
    }

    private bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            default: return true;
            case Sound.Crank_Turn:
            case Sound.Tank_Exhaust:
            case Sound.Tank_Move_Creak:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float timerMax = GetAudioClip(sound).length;
                    if (lastTimePlayed + timerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    } else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
        }
    }

    public void UpdateVolumes(Category category)
    {
        foreach(AudioSource source in GetAudioSourcesOfCategory(category))
        {
            source.volume = GetVolumeSetting(category) * categoryVolumeModifiers[category];
        }
    }

    private List<AudioSource> GetAudioSourcesOfCategory(Category category)
    {
        List<AudioSource> result = new List<AudioSource>();

        foreach(AudioHelper audioHelper in audioHelperList)
        {
            if(audioHelper.audioCategory == category)
                result.Add(audioHelper.GetComponent<AudioSource>());
        }

        return result;
    }

    public void MuteCategory(Category category, bool mute)
    {
        foreach(AudioSource audioSource in GetAudioSourcesOfCategory(category))
        {
            audioSource.mute = mute;
        }
    }
}