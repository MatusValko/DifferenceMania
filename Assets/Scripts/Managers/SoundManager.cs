using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    LOGIN_THEME,
    MAIN_MENU_THEME,
    PREMIUM_THEME,
    BUTTON_CLICK,
}



// [RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
    public static SoundManager Instance { get; private set; }//RENAME
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //check if GO is destroyed

            Debug.LogWarning("SOUND MANAGER IS INSTANTIATED");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }




    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlaySound(SoundType.LOGIN_THEME);
    }


    public static void PlaySound(SoundType sound, AudioSource source = null, float volume = 1)
    {
        // Instance.audioSource.PlayOneShot(Instance.soundList[(int)sound]);
    }

    // public static void PlaySound(SoundType sound, AudioSource source = null, float volume = 1)
    // {
    //     SoundList soundList = Instance.SO.sounds[(int)sound];
    //     AudioClip[] clips = soundList.sounds;
    //     AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

    //     if (source)
    //     {
    //         source.outputAudioMixerGroup = soundList.mixer;
    //         source.clip = randomClip;
    //         source.volume = volume * soundList.volume;
    //         source.Play();
    //     }
    //     else
    //     {
    //         Instance.audioSource.outputAudioMixerGroup = soundList.mixer;
    //         Instance.audioSource.PlayOneShot(randomClip, volume * soundList.volume);
    //     }
    // }
    // Update is called once per frame


#if UNITY_EDITOR
    private void OnEnable()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif
}


[Serializable]
public struct SoundList
{
    public string name;
    [Range(0, 1)] public float volume;
    // public AudioMixerGroup mixer;
    public AudioClip[] sounds;
}

