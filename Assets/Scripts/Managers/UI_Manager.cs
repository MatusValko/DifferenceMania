using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance;

    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private TextMeshProUGUI _refillLivesText; // Text for refill lives button

    [SerializeField] private TextMeshProUGUI _coinsText;

    [SerializeField] private TextMeshProUGUI _profileLevelText;
    [SerializeField] private Image _profileAvatarImage;
    [SerializeField] private GameObject _lockedLevelText;
    [SerializeField] private GameObject _levelSelectWindow;


    // [Header("Avatars Data")]
    // public ProfileOneAvatarImage[] avatars; // Array to hold avatar GameObjects

    // [SerializeField] private ProfileOneAvatarImage _oneAvatarPrefab; // Index of the currently selected avatar
    // [SerializeField] private Transform _contentTransform; // Transform for the content area where avatars will be instantiated

    [Header("Level Select Window")]

    public GameObject Content;

    public GameObject EpisodePrefab; // Prefab for the level button
    public GameObject LevelPrefab; // Prefab for the level button
    public GameObject LockedLevelPrefab; // Prefab for the level button



    //generate levels based on level data from game manager
    public IEnumerator GenerateLevelsAsync()
    {
        List<EpisodeData> episodes;

        yield return StartCoroutine(LevelLoader.GetProgressData());
        // After loading progress data, try to get episodes again
        episodes = GameManager.Instance.GetEpisodes();
        if (episodes == null || episodes.Count == 0)
        {
            yield break;
        }

        // Loop through the level data and create buttons for each level
        foreach (EpisodeData episodeData in episodes)
        {
            // instantiate the episode prefab
            GameObject episodeGO = Instantiate(EpisodePrefab);
            //set parent to content
            episodeGO.transform.SetParent(Content.transform, false);
            Episode episode = episodeGO.GetComponent<Episode>();
            episode.SetName("Episode " + episodeData.id);

            if (GameManager.Instance.GetStarsCollected() >= episodeData.unlock_stars)
            {
                // DebugLogger.Log($"Episode {episodeData.id} unlocked with {GameManager.Instance.GetStarsCollected()} stars");
                episode.SetLockedTextOff();
                if (episodeData.unlock_coins == 0)
                {
                    episode.SetLockedButtonOFF();
                }
                else
                {
                    episode.SetLockedButton(episodeData.unlock_coins);
                }

                foreach (LevelData levelData in episodeData.levels)
                {
                    //write all levelData parameters to debug log
                    // DebugLogger.Log($"Level ID: {levelData.id}, Name: {levelData.name}, Stars: {levelData.stars_collected}, Opened: {levelData.opened}");
                    if (levelData.opened || levelData.id == 1)
                    {
                        // Create a new button for the level
                        GameObject levelGO = Instantiate(LevelPrefab);
                        episode.AddLevel(levelGO);
                        Level level = levelGO.GetComponent<Level>();
                        level.SetLevelNumber(levelData.name);
                        level.SetStars(levelData.stars_collected);
                        level.SetOnClickEvent(levelData.id);
                    }
                    else
                    {
                        GameObject lockedLevelGO = Instantiate(LockedLevelPrefab);
                        Level level = lockedLevelGO.GetComponent<Level>();
                        level.SetLockedLevelEvent();
                        episode.AddLevel(lockedLevelGO);
                    }

                }
            }
            else
            {
                // DebugLogger.Log($"Episode {episodeData.id} locked with {GameManager.Instance.GetStarsCollected()} stars");
                episode.SetLockedText(GameManager.Instance.GetStarsCollected(), episodeData.unlock_stars);
                episode.SetLockedTextOn();
                // episode.SetLockedButton(episodeData.unlock_coins);
                episode.SetLockedButtonOFF();
                foreach (LevelData levelData in episodeData.levels)
                {
                    // Create a new button for the level
                    GameObject lockedLevelGO = Instantiate(LockedLevelPrefab);
                    Level level = lockedLevelGO.GetComponent<Level>();
                    level.SetLockedLevelEvent();
                    episode.AddLevel(lockedLevelGO);
                }
            }
        }

    }

    public void ShowLockedLevelText()
    {
        // Debug.Log("Showing locked level text");
        if (_lockedLevelText != null)
        {
            // Debug.Log("Locked level text is not null");
            if (_lockedLevelText.activeSelf)
            {
                // Debug.Log("Locked level text is already active");
                return; // If the text is already active, do nothing
            }
            _lockedLevelText.SetActive(true);
            //set off after 2 seconds
            // StartCoroutine(HideLockedLevelTextAfterDelay(2.5f));
        }
    }

    public void ResetLockedLevelText()
    {
        if (!_lockedLevelText.activeSelf)
        {
            return;
        }
        _lockedLevelText.GetComponent<Animator>().Play("Default");
        _lockedLevelText.SetActive(false);
    }

    public void UpdateRefillLivesUI()
    {
        if (_refillLivesText != null)
        {
            _refillLivesText.text = GameManager.Instance.GetLives().ToString();
        }
    }
    public void UpdateLivesUI()
    {
        if (_livesText != null)
        {
            if (GameManager.Instance.GetLives() == GameManager.Instance.GetMaxLiveConst())
            {
                _livesText.text = "FULL";
            }
            else
            {
                _livesText.text = $"{GameManager.Instance.GetLives() + "/" + GameManager.Instance.GetMaxLiveConst()}";
            }
            // _livesText.text = $"{GameManager.Instance.GetLives() + "/" + GameManager.Instance.GetMaxLiveConst()}";
        }
    }
    public void UpdateCoinsUI()
    {
        //if not null
        if (_coinsText != null)
        {
            _coinsText.text = $"{GameManager.Instance.GetCoins()}";
        }
    }
    public void UpdateProfileLevelAndAvatar()
    {
        if (_profileLevelText == null || _profileAvatarImage == null)
        {
            Debug.LogError("Profile UI elements are not assigned.");
            return;
        }
        _profileLevelText.text = $"LVL {GameManager.Instance.GetProfileLevel()}";
        _profileAvatarImage.sprite = GameManager.Instance.GetCurrentProfileAvatarSprite();
    }

    // public Sprite GetCurrentProfileAvatarSprite()
    // {
    //     int i = GameManager.Instance.GetSelectedPFP() - 1;
    //     _currentProfileAvatarSprite = avatarSprites[i];
    //     if (_currentProfileAvatarSprite == null)
    //     {
    //         DebugLogger.LogError("Current profile avatar sprite is not set.");
    //         return null;
    //     }
    //     // Return the current profile avatar sprite
    //     return _currentProfileAvatarSprite;
    // }
    public IEnumerator InitializeUI()
    {
        // Wait for GameManager to be ready
        yield return new WaitUntil(() => GameManager.Instance.GetToken() != null && GameManager.Instance.GetToken() != "");
        UpdateCoinsUI();
        UpdateLivesUI();
        UpdateRefillLivesUI();

        UpdateProfileLevelAndAvatar();
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }



    void Start()
    {
        GameManager.Instance.SetFadeToActive(); // Reset level ID to 1 when going back to main menu
        if (GameManager.Instance.ShowLevelsAfterPlaying()) _levelSelectWindow.SetActive(true);
        StartCoroutine(InitializeUI());
        //wait until levels are generated through GenerateLevelsAsync()
        StartCoroutine(WaitForLevelsGenerated());
        // StartCoroutine(GenerateLevelsAsync());
        // SoundManager.PlayThemeSound(SoundType.MAIN_MENU_THEME, 0.7f); //IF QUICKLY LOADED THIS WILL MAKE SURE THE MUSIC IS PLAYING
    }

    private IEnumerator WaitForLevelsGenerated()
    {
        yield return StartCoroutine(GenerateLevelsAsync());
        GameManager.Instance.FadeInLevel();
    }

    //on enable
    private void OnEnable()
    {
        InitializeUI();
    }
}
