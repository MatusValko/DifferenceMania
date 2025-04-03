using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IDataPersistenceManager
{
    public int MAX_LIVES = 3;
    public const int MAX_LIVES_COUNT_CONST = 5;

    public bool ISLOGGEDIN = false;
    public const string GAMESERVER = "https://diff.nconnect.sk";
    public const string API_REGISTER = "https://diff.nconnect.sk/api/register";

    public const string API_LOGIN = "https://diff.nconnect.sk/api/login";
    public const string API_DIFF_IMAGES = "https://diff.nconnect.sk/api/diff-iamges";

    public const string API_LOAD_USER_DATA = "https://diff.nconnect.sk/api/load_user_data";
    public const string API_GET_USER_LEVEL_DATA = "https://diff.nconnect.sk/api/progress";



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
    [SerializeField]
    private List<int> _unlockedPFP;
    [SerializeField] private int _boostAddTimeCount;
    [SerializeField] private int _boostHintCount;

    [SerializeField] private bool has_ads_removed;
    [SerializeField] private bool _rewardedForAccConnection;
    [SerializeField] private bool _hasAdsRemoved;
    [SerializeField] private int last_refill_timestamp;
    [SerializeField] private int? _life_refill_time;

    [SerializeField] private const int BUY_HINT_PRICE = 10;

    [SerializeField] public const int WINS_NEED_TO_GIFT = 6;




    [Header("Account details")]
    [SerializeField]
    private string _token;
    [SerializeField]
    private string _nickname;
    [SerializeField]
    private string _email;
    [SerializeField]
    private bool _hasFreeNickName;
    [SerializeField]
    private string _device_name;
    // [SerializeField]
    // private string _password;

    [Header("Level data")]
    [SerializeField]
    private List<EpisodeData> _episodes;
    private int _levelID = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("GAME MANAGER IS INSTANTIATED");
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
        // CheckIfIsLoggedIn();
        // _device_name = SystemInfo.deviceModel;
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
        // if (string.IsNullOrEmpty(_email))
        // {
        //     Debug.LogWarning("Email is empty!");
        //     Instance.ISLOGGEDIN = false;
        //     return;
        // }
        Debug.Log("Has token and email. Logging in");
        Instance.ISLOGGEDIN = true;
    }

    //get level data url adress
    public string GetLevelDataURL(int levelID)
    {
        string url = $"https://diff.nconnect.sk/api/level/{levelID}";
        return url;
    }

    public void AddLive(int live = 1)
    {
        if (_lives == MAX_LIVES)
        {
            return;
        }
        _lives += live;
        UI_Manager.Instance.UpdateLives();
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
    public void AddCoins(int coin)
    {
        _coins += coin;
        UI_Manager.Instance.UpdateCoins();
    }
    // get level data
    public List<EpisodeData> GetEpisodes()
    {
        return _episodes;
    }
    //set level data
    public void SetEpisodes(List<EpisodeData> episodes)
    {
        _episodes = episodes;
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
        return _token;
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
    public int GetUnlockedLevels()
    {
        return _unlockedLevels;
    }
    public int GetSelectedPFP()
    {
        return _selectedPFP;
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
    //set current wins
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
    }
    public void SetUnlockedPFP(int unlockedPFP)
    {
        _unlockedPFP.Add(unlockedPFP);
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
        has_ads_removed = hasAdsRemoved;
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
}
