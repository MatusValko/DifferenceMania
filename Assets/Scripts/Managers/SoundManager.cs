using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




// [RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundList[] soundList;
    public static SoundManager Instance { get; private set; }//RENAME
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private Queue<AudioClip> audioQueue = new Queue<AudioClip>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
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
        // PlaySound(SoundType.LOGIN_THEME);
    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        AudioClip[] clips = Instance.soundList[(int)sound].Sounds;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        Instance._sfxSource.PlayOneShot(randomClip, volume);
    }

    public static void PlayThemeSound(SoundType sound, float volume = 1)
    {
        //if there is a clip playing with the same sound type, don't play it again
        if (Instance._musicSource.clip != null)
        {
            //check if the clip is from the same soundType, search all clips in the same soundList copilot
            foreach (AudioClip clip in Instance.soundList[(int)sound].Sounds)
            {
                if (Instance._musicSource.clip == clip)
                {
                    return;
                }
            }
        }
        Instance.StopTheme();

        AudioClip[] clips = Instance.soundList[(int)sound].Sounds;
        List<AudioClip> shuffledClips = new List<AudioClip>(clips);
        Shuffle(shuffledClips);

        foreach (AudioClip clip in shuffledClips)
        {
            Instance.audioQueue.Enqueue(clip);
        }
        Instance.PlayNextClip(volume);
    }

    private static void Shuffle(List<AudioClip> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            AudioClip value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void PlayNextClip(float volume)
    {
        if (audioQueue.Count > 0)
        {
            AudioClip clip = audioQueue.Dequeue();
            _musicSource.clip = clip;
            _musicSource.Play();
            // _musicSource.PlayOneShot(clip, volume);
            StartCoroutine(WaitForClipToEnd(clip.length, volume));
        }
    }

    //reset music source clip to null copilot
    public void StopTheme()
    {
        _musicSource.Stop();
        _musicSource.clip = null;
    }

    private IEnumerator WaitForClipToEnd(float clipLength, float volume)
    {
        yield return new WaitForSeconds(clipLength);
        PlayNextClip(volume);
    }

    public void ToggleMuteMusic(bool mute)
    {
        _musicSource.mute = mute;
    }

    public void ToggleMuteSFX(bool mute)
    {
        _sfxSource.mute = mute;
    }

    public bool IsMusicMuted()
    {
        return _musicSource.mute;
    }
    public bool IsSFXMuted()
    {
        return _sfxSource.mute;
    }

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

[System.Serializable]
public struct SoundList
{
    public AudioClip[] Sounds { get => sounds; }
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
}

public enum SoundType
{
    LOGIN_THEME,
    MAIN_MENU_THEME,
    PREMIUM_THEME,
    PREMIUM_START_COIN,
    PREMIUM_START_REGISTER,
    COLLECTION_START_OPEN_WINDOW,
    GAME_THEME,
    GAME_INCORRECT_CLICK,
    GAME_CORRECT_CLICK,
    GAME_CORRECT_HINT,
    GAME_LOSE,
    GAME_WIN,
    GAME_TIME,
    CONGRATULATION_CONFETTI,
    CONGRATULATION_FANFARE,
    GIFT_OPEN,
    BUTTON_CLICK,

}

