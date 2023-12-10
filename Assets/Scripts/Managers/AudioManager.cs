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
    }

    private void Start()
    {
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

        audioHelperList = new List<AudioHelper>();

        UpdateVolumeSettings();
    }

    public void UpdateVolumeSettings()
    {
        Debug.Log("Updating volume settings in AudioManager");

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

            audioHelper.audioCategory = GetCategory(sound);

            if(!audioHelperList.Contains(audioHelper))
                audioHelperList.Add(audioHelper);

            oneShotAudioSource.volume = GetVolumeSetting(GetCategory(sound)) * volume;
            oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
        }
    }

    public void PlaySound(Sound sound, Vector3 position, float volume = 1f)
    {
        if (CanPlaySound(sound))
        {
            GameObject soundGameObject = new GameObject("Sound");
            soundGameObject.transform.position = position;

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
            audioSource.volume = GetVolumeSetting(GetCategory(sound)) * volume;
            audioSource.Play();

            Object.Destroy(soundGameObject, audioSource.clip.length);
        }
    }

    public AudioSource AttachConstantSound(Sound sound, Transform transform, float volume = 1f)
    {
        GameObject soundGameObject = new GameObject("Sound");
        soundGameObject.transform.SetParent(transform);
        soundGameObject.transform.localPosition = Vector3.zero;

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
        audioSource.volume = GetVolumeSetting(GetCategory(sound)) * volume;

        audioSource.loop = true;
        audioSource.Play();

        return audioSource;
    }

    public void PlayMusic(Music music, float volume = 1f)
    {
        float fadeDuration = 3f;

        if(musicPlayingGameObject != null && musicPlayingAudioSource != null)
        {
            if (musicPlayingAudioSource.clip == GetAudioClip(music))
                return;

            musicPlayingAudioSource.GetComponent<AudioHelper>().StartCoroutine(
                StartFade(musicPlayingAudioSource, fadeDuration, 0f));
        }

        GameObject musicGameObject = new GameObject("Music");

        AudioHelper audioHelper = musicGameObject.AddComponent<AudioHelper>();
        audioHelper.audioCategory = GetCategory(music);
        audioHelper.SetDontDestroyOnLoad(true);
        audioHelper.containingList = audioHelperList;
        audioHelperList.Add(audioHelper);

        AudioSource audioSource = musicGameObject.AddComponent<AudioSource>();
        audioSource.clip = GetAudioClip(music);
        audioSource.maxDistance = 100f;
        audioSource.spatialBlend = 1f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.dopplerLevel = 0f;
        audioSource.volume = GetVolumeSetting(GetCategory(music)) * volume;
        audioSource.loop = true;

        audioSource.Play();

        audioHelper.StartCoroutine(StartFade(audioSource, fadeDuration, GetVolumeSetting(GetCategory(music)) * volume));

        musicPlayingGameObject = musicGameObject;
        musicPlayingAudioSource = audioSource;
    }

    public IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        Debug.Log("Started fading!");

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
            //Debug.Log("Updating " + source.gameObject.name + " to " + volume);
            source.volume = GetVolumeSetting(category);
        }
    }

    private List<AudioSource> GetAudioSourcesOfCategory(Category category)
    {
        List<AudioSource> result = new List<AudioSource>();

        Debug.Log("AudioHelperList - " + audioHelperList);

        foreach(AudioHelper audioHelper in audioHelperList)
        {
            Debug.Log("AudioHelperList -> " + audioHelper);

            if(audioHelper.audioCategory == category)
                result.Add(audioHelper.GetComponent<AudioSource>());
        }

        return result;
    }
}
