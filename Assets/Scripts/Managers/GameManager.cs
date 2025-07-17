using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistenceManager
{
    public int MAX_LIVES = 3;
    public const int MAX_LIVES_COUNT_CONST = 5;

    public bool ISLOGGEDIN = false;
    public const string GAMESERVER = GameConstants.GAMESERVER;
    public const string API_REGISTER = GameConstants.API_REGISTER;
    public const string API_LOGIN = GameConstants.API_LOGIN;
    public const string API_DIFF_IMAGES = GameConstants.API_DIFF_IMAGES;
    public const string API_LOAD_USER_DATA = GameConstants.API_LOAD_USER_DATA;
    public const string API_GET_USER_LEVEL_DATA = GameConstants.API_GET_USER_LEVEL_DATA;

    public static GameManager Instance { get; private set; }//RENAME

    [Header("Account game data")]
    [SerializeField]
    private int _coins;
    [SerializeField]
    private int _lives;
    [SerializeField]
    private int _starsCollected;
    [SerializeField]
    private int _experience;
    [SerializeField]
    private int _experienceToNextLevel;

    [SerializeField]
    private int _unlockedLevels;
    [SerializeField]
    private int _finishedLevels;
    [SerializeField]
    private int _currentWins;
    [SerializeField]
    private int _selectedPFP;
    [SerializeField]
    private int _profileLevel;
    [SerializeField] private int _boostAddTimeCount;
    [SerializeField] private int _boostHintCount;

    [SerializeField] private bool _hasAdsRemoved;
    [SerializeField] private bool _rewardedForAccConnection;
    [SerializeField] private int last_refill_timestamp;
    [SerializeField] private int? _life_refill_time;

    [SerializeField] private const int BUY_HINT_PRICE = 10;

    [SerializeField] public const int WINS_NEEDED_TO_GIFT = 6;

    [Header("Account details")]
    [SerializeField] private string _token;
    [SerializeField] private string _nickname;
    [SerializeField] private string _email;
    [SerializeField] private bool _hasFreeNickName;
    [SerializeField] private string _device_name;
    [SerializeField] private string _playerID;

    // [SerializeField]
    // private string _password;

    [Header("Level data")]
    [SerializeField] private List<EpisodeData> _episodes;
    [SerializeField] private int _levelID = 1;
    [SerializeField] private bool _showLevelsAfterPlaying = false;


    [Header("Avatars Data")]
    [SerializeField] private Sprite[] _avatarSprites; // Array to hold avatar sprites
    [SerializeField] private Sprite[] _avatarBackgroundSprites; // Array to hold avatar sprites

    [SerializeField] private List<int> _unlockedPFP;
    [SerializeField] private Sprite _currentProfileAvatarSprite;
    [SerializeField] private Sprite _currentProfileBackgroundSprite;

    // [SerializeField] private int[] _grayBackgroundIndexes = new int[] { 1, 2, 3, 4, 5,7,8,9,11, }; // Array to hold indexes of gray background sprites
    [SerializeField] private int[] _greenBackgroundIndexes = new int[] { 6, 10, 13, 17, 22 }; // Array to hold indexes of gray background sprites
    [SerializeField] private int[] _purpleBackgroundIndexes = new int[] { 14, 23, 20, 25, 24 }; // Array to hold indexes of gray background sprites

    [Header("Collection Data")]
    [SerializeField] private Sprite[] _collectionItemSprites;
    [SerializeField] private Sprite[] _collectionItemSpritesBLACKnWHITE;




    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            // Debug.LogWarning("GAME MANAGER IS INSTANTIATED");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }
    private void Start()
    {
        _setTargetFrameRate();
        CheckIfIsLoggedIn();
        _device_name = SystemInfo.deviceModel;
    }

    //set target frame rate to 60
    private void _setTargetFrameRate()
    {
        Application.targetFrameRate = 60;
    }

    public void CheckIfIsLoggedIn()
    {
        if (string.IsNullOrEmpty(_token))
        {
            Debug.LogWarning("Token is empty!");
            Instance.ISLOGGEDIN = false;
            return;
        }
        Debug.Log("Has token and email. Logging in");
        Instance.ISLOGGEDIN = true;
    }
    //TODO USE THIS INSTEAD
    public bool CheckIfIsLoggedInNEW()
    {
        if (string.IsNullOrEmpty(_token))
        {
            Debug.LogWarning("Token is empty!");
            // Instance.ISLOGGEDIN = false;
            return false;
        }
        // if (string.IsNullOrEmpty(_email))
        // {
        //     Debug.LogWarning("Email is empty!");
        //     Instance.ISLOGGEDIN = false;
        //     return;
        // }
        Debug.Log("Has token and email. Logging in");
        return true;
        // Instance.ISLOGGEDIN = true;
    }

    //get level data url adress
    public string GetLevelDataURL(int levelID)
    {
        string url = $"{GAMESERVER}/api/level/{levelID}";
        return url;
    }


    //get player ID
    public string GetPlayerID()
    {
        if (string.IsNullOrEmpty(_playerID))
        {
            Debug.LogWarning("Player ID is empty!");
            return "Unknown Player";
        }
        return _playerID;
    }
    public void SetPlayerID(string playerID)
    {
        _playerID = playerID;
    }
    public void AddLive(int live = 1)
    {
        if (_lives == MAX_LIVES)
        {
            return;
        }
        _lives += live;
        UI_Manager.Instance.UpdateLivesUI();
    }
    public void AddCoins(int coin)
    {
        _coins += coin;
        UI_Manager.Instance.UpdateCoinsUI();
    }
    //check if has enough coins to buy something
    public bool HasEnoughCoins(int price)
    {
        if (_coins >= price)
        {
            return true;
        }
        //play animation
        if (UI_Manager.Instance == null)
        {
            DebugLogger.LogError("UI_Manager is null. Cannot play not enough coins animation.");
            return false;
        }
        //get animator from tag 
        GameObject animatorObj = GameObject.FindGameObjectWithTag("TopUI");
        if (animatorObj == null) return false;
        Animator animator = animatorObj.GetComponent<Animator>();
        if (animator == null)
        {
            DebugLogger.LogError("Animator is null. Cannot play not enough coins animation.");
            return false;
        }
        animator.SetTrigger("NotEnoughCoins");
        //play animation
        return false;
    }
    //get hints and time
    public int GetBoostAddTimeCount()
    {
        return _boostAddTimeCount;
    }
    public int GetBoostHintCount()
    {
        return _boostHintCount;
    }

    // get level data
    public List<EpisodeData> GetEpisodes()
    {
        return _episodes;
    }
    public void SetEpisodes(List<EpisodeData> episodes)
    {
        _episodes = episodes;
    }

    public Sprite[] GetCollectionItemSprites()
    {
        return _collectionItemSprites;
    }
    public Sprite[] GetCollectionItemSpritesBlacknWhite()
    {
        return _collectionItemSpritesBLACKnWHITE;
    }
    //set level id
    public void SetLevelID(int levelID)
    {
        _levelID = levelID;
    }
    //get level id
    public int GetLevelID()
    {
        return _levelID;
    }
    public int GetCurrentWins()
    {
        return _currentWins;
    }
    public int GetLives()
    {
        return _lives;
    }
    public int GetCoins()
    {
        return _coins;
    }
    //get token
    public string GetToken()
    {
        if (string.IsNullOrEmpty(_token))
        {
            DebugLogger.LogWarning("Token is empty!");
            return null;
        }
        // DebugLogger.Log($"Token: {_token}");
        return _token;
    }
    //TODO IMPLEMENT GETTING PROFILE IMAGE
    public Sprite GetProfileImageSprite()
    {

        return Resources.Load<Sprite>($"ProfileLevels/Level{_profileLevel}");
    }
    public void SetToken(string token)
    {
        _token = token;
    }

    public void SetEmail(string email)
    {
        _email = email;
    }
    public void SetStarsCollected(int starsCollected)
    {
        _starsCollected = starsCollected;
    }
    public void SetFinishedLevels(int finishedLevels)
    {
        _finishedLevels = finishedLevels;
    }
    public int GetFinishedLevels()
    {
        return _finishedLevels;
    }
    public int GetBoostAddTimePrice()
    {
        return _boostAddTimeCount;
    }
    public int GetBoostHintPrice()
    {
        return _boostHintCount;
    }
    //set experience
    public void SetExperience(int experience)
    {
        _experience = experience;
    }
    //set experience to next level
    public void SetExperienceToNextLevel(int experience)
    {
        _experienceToNextLevel = experience;
    }

    public int GetStarsCollected()
    {
        return _starsCollected;
    }
    public int GetExperience()
    {
        return _experience;
    }
    public int GetExperienceToNextLevel()
    {
        return _experienceToNextLevel;
    }
    public int GetUnlockedLevels()
    {
        return _unlockedLevels;
    }
    public int GetSelectedPFP()
    {
        return _selectedPFP;
    }
    public Sprite GetCurrentProfileAvatarSprite()
    {
        if (_currentProfileAvatarSprite == null)
        {
            DebugLogger.LogError("Current profile avatar sprite is not set. Setting Default Avatar.");
            _currentProfileAvatarSprite = _avatarSprites[0]; // Set to default avatar if not set
            _currentProfileBackgroundSprite = _avatarBackgroundSprites[0]; // Set to default background if not set
            return _currentProfileAvatarSprite;
        }
        // Return the current profile avatar sprite
        return _currentProfileAvatarSprite;
    }

    public int GetProfileLevel()
    {
        return _profileLevel;
    }
    public int GetMaxLives()
    {
        return MAX_LIVES;
    }
    public void SetDeviceName(string device_name)
    {
        _device_name = device_name;
    }
    //set rewarded for account connection
    public void SetRewardedForAccountConnection(bool rewardedForAccConnection)
    {
        _rewardedForAccConnection = rewardedForAccConnection;
    }
    public void SetNickname(string nickname)
    {
        _nickname = nickname;
    }
    //set max lives
    public void SetMaxLives(int maxLives)
    {
        MAX_LIVES = maxLives;
    }
    public void SetProfileLevel(int level)
    {
        _profileLevel = level;
    }
    public void SetBoostAddTime(int boostAddTime)
    {
        _boostAddTimeCount = boostAddTime;
    }
    //set life refill time
    public void SetLifeRefillTime(int? lifeRefillTime)
    {
        _life_refill_time = lifeRefillTime;
    }
    public string GetNickname()
    {
        return _nickname;
    }
    public string GetEmail()
    {
        return _email;
    }
    public bool GetFreeNickName()
    {
        return _hasFreeNickName;
    }
    public void SetFreeNickName(bool freeNickName)
    {
        _hasFreeNickName = freeNickName;
    }
    public void SetUnlockedLevels(int unlockedLevels)
    {
        _unlockedLevels = unlockedLevels;
    }

    //get Max lives, TODO FIX THIS 
    public int GetMaxLiveConst()
    {
        return MAX_LIVES_COUNT_CONST;
    }
    //set current wins for GIFT ROOM
    public void SetCurrentWins(int currentWins)
    {
        _currentWins = currentWins;
    }
    //set boost hint
    public void SetBoostHint(int boostHint)
    {
        _boostHintCount = boostHint;
    }
    public void SetSelectedPFP(int selectedPFP)
    {
        _selectedPFP = selectedPFP;
        // GET CURRENT PROFILE AVATAR SPRITE
        int i = selectedPFP - 1;
        if (i >= 0 && i < _avatarSprites.Length)
        {
            _currentProfileAvatarSprite = _avatarSprites[i];
            // _currentProfileBackgroundSprite = SetCurrentProfileBackgroundSprite(i + 1);
        }
        else
        {
            DebugLogger.LogError("Selected PFP index is out of bounds.");
        }
    }
    public Sprite GetProfileBackgroundSprite(int index)
    {
        //0 SELECTED, 1 GRAY, 2 GREEN, 3 PURPLE
        if (index < 0 || index >= _avatarSprites.Length)
        {
            DebugLogger.LogError("Avatar background sprite index is out of bounds.");
            return null;
        }
        if (_greenBackgroundIndexes.Contains(index))
        {
            // _currentProfileBackgroundSprite = _avatarBackgroundSprites[1];
            return _avatarBackgroundSprites[2]; // Set to green background if in green indexes
        }
        else if (_purpleBackgroundIndexes.Contains(index))
        {
            // _currentProfileBackgroundSprite = _avatarBackgroundSprites[2];
            return _avatarBackgroundSprites[3]; // Set to purple background if in purple indexes
        }
        else
        {
            // Default to gray background if not in green or purple indexes
            // _currentProfileBackgroundSprite = _avatarBackgroundSprites[0];
            return _avatarBackgroundSprites[1]; // Set to gray background if not in green or purple indexes
        }
    }
    //get avatar sprites length
    public int GetAvatarSpritesLength()
    {
        return _avatarSprites.Length;
    }
    //return avatar sprite based on index
    public Sprite GetAvatarSprite(int index)
    {
        if (index < 0 || index >= _avatarSprites.Length)
        {
            DebugLogger.LogError("Avatar sprite index is out of bounds.");
            return null;
        }
        return _avatarSprites[index];
    }
    public void SetUnlockedPFP(List<int> list)
    {
        _unlockedPFP = list;
    }
    public List<int> GetUnlockedPFP()
    {
        return _unlockedPFP;
    }
    public int GetUnlockedPFPCount()
    {
        return _unlockedPFP.Count;
    }
    //Set has ads removed
    public void SetHasAdsRemoved(bool hasAdsRemoved)
    {
        _hasAdsRemoved = hasAdsRemoved;
    }
    //set free nickname available
    public void SetFreeNickNameAvailable(bool freeNickNameAvailable)
    {
        _hasFreeNickName = freeNickNameAvailable;
    }
    //set last refill timestamp
    public void SetLastRefillTimestamp(int lastRefillTimestamp)
    {
        last_refill_timestamp = lastRefillTimestamp;
    }

    // public void SetFreeNickNameToTrue()
    // {
    //     _hasFreeNickName = true;
    // }
    public void SetCoins(int coins)
    {
        _coins = coins;
    }
    public void SetLives(int lives)
    {
        _lives = lives;
    }


    public void AddWin()
    {
        _currentWins += 1;
    }
    public void ResetWins()
    {
        _currentWins = 0;
    }
    public void LogOut()
    {
        SetEmail(null);
        SetToken(null);
        SetNickname(null);
    }
    public void SetShowLevelsAfterPlaying(bool showLevels)
    {
        _showLevelsAfterPlaying = showLevels;
    }

    public bool ShowLevelsAfterPlaying()
    {
        if (_showLevelsAfterPlaying)
        {
            _showLevelsAfterPlaying = false;
            return true;
        }
        return false;
    }

    public void LoadData(GameData gameData)
    {
        _coins = gameData.Coins;
        _lives = gameData.Lives;
        _unlockedLevels = gameData.UnlockedLevels;
        _unlockedPFP = gameData.UnlockedPFP;
        _selectedPFP = gameData.SelectedPFP;
        _experience = gameData.Experience;
        _currentWins = gameData.CurrentWins;

        _token = gameData.Token;
        _playerID = gameData.PlayerID;
        _email = gameData.Email;
        _nickname = gameData.Nickname;
        _hasFreeNickName = gameData.HasFreeNickName;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.Coins = _coins;
        gameData.Lives = _lives;
        gameData.UnlockedLevels = _unlockedLevels;
        gameData.UnlockedPFP = _unlockedPFP;
        gameData.SelectedPFP = _selectedPFP;
        gameData.Experience = _experience;
        gameData.CurrentWins = _currentWins;


        gameData.Token = _token;
        gameData.PlayerID = _playerID;
        gameData.Email = _email;
        gameData.Nickname = _nickname;
        gameData.HasFreeNickName = _hasFreeNickName;
    }

    // UseBoostHint function
    public void UseBoostHint()
    {
        if (_boostHintCount > 0)
        {
            _boostHintCount--;
            // UI_Manager.Instance.UpdateBoostHint();
        }
    }
    // function UseBoostAddTime
    public void UseBoostAddTime()
    {
        if (_boostAddTimeCount > 0)
        {
            _boostAddTimeCount--;
            // UI_Manager.Instance.UpdateBoostAddTime();
        }
    }
    public bool BuyBoostHint()
    {
        // _boostHintCount++;
        if (_coins >= BUY_HINT_PRICE)
        {
            _coins -= BUY_HINT_PRICE;
            return true;
        }
        return false;
    }

    public void LoadLevel(int levelID)
    {
        Instance.SetLevelID(levelID);
        ScreenFader fader = FindFirstObjectByType<ScreenFader>();
        if (fader == null)
        {
            Debug.LogError("ScreenFader not found in the scene.");
            return;
        }
        fader.FadeOut(3);
    }
    public void FadeInLevel()
    {
        ScreenFader fader = FindFirstObjectByType<ScreenFader>();
        if (fader == null)
        {
            Debug.LogError("ScreenFader not found in the scene.");
            return;
        }
        fader.gameObject.SetActive(true);
        fader.FadeIn();
    }

    public void FadeOutToMainMenu()
    {
        ScreenFader fader = FindFirstObjectByType<ScreenFader>();
        fader.FadeOut(1);
    }

    public void SetFadeToActive()
    {
        ScreenFader fader = FindFirstObjectByType<ScreenFader>();
        if (fader == null)
        {
            Debug.LogError("ScreenFader not found in the scene.");
            return;
        }
        fader.SetToBlackScreen();
    }

}
