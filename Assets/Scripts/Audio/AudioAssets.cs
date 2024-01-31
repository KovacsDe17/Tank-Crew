using UnityEngine;
using static AudioManager;

/// <summary>
/// This class is for holding audio assets for the AudioManager.
/// </summary>
public class AudioAssets : MonoBehaviour
{
    public static AudioAssets _i;

    //If the assets are not cached, instantiate their holding object from the resources.
    public static AudioAssets i {  
        get { 
            if( _i == null ) _i = Instantiate(Resources.Load("AudioAssets") as GameObject).GetComponent<AudioAssets>();
            return _i; 
        } 
    }

    public SoundObject[] audioArray;

    [System.Serializable]
    public class SoundObject
    {
        public Sound sound;
        public AudioClip clip;
    }

    public MusicObject[] musicArray;

    [System.Serializable]
    public class MusicObject
    {
        public Music music;
        public AudioClip clip;
    }
}