using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IDataPersistenceManager
{
    public const int MAX_LIVES = 3;

    public bool ISLOGGEDIN = false;
    public const string GAMESERVER = "https://diff.nconnect.sk";
    public const string API_REGISTER = "https://diff.nconnect.sk/api/register";

    public const string API_LOGIN = "https://diff.nconnect.sk/api/login";
    public const string API_DIFF_IMAGES = "https://diff.nconnect.sk/api/diff-iamges";



    public static GameManager Instance { get; private set; }//RENAME

    [Header("Account game data")]
    [SerializeField]
    private int _coins;
    [SerializeField]
    private int _lives;
    [SerializeField]
    private int _experience;
    [SerializeField]
    private int _unlockedLevels;
    [SerializeField]
    private int _selectedPFP;
    [SerializeField]
    private List<int> _unlockedPFP;

    [Header("Account details")]
    [SerializeField]
    private string _token;
    [SerializeField]
    private string _nickname;
    [SerializeField]
    private string _email;
    [SerializeField]
    private bool _hasFreeNickName;
    // [SerializeField]
    // private string _device_name;
    // [SerializeField]
    // private string _password;



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
        CheckIfIsLoggedIn();


        // _device_name = SystemInfo.deviceModel;
    }

    public void CheckIfIsLoggedIn()
    {
        if (string.IsNullOrEmpty(_token))
        {
            Debug.LogWarning("Token is empty!");
            Instance.ISLOGGEDIN = false;
            return;
        }
        if (string.IsNullOrEmpty(_email))
        {
            Debug.LogWarning("Email is empty!");
            Instance.ISLOGGEDIN = false;
            return;
        }
        Debug.Log("Has token and email. Logging in");
        Instance.ISLOGGEDIN = true;
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
    public void AddCoins(int coin)
    {
        _coins += coin;
        UI_Manager.Instance.UpdateCoins();
    }
    public int GetLives()
    {
        return _lives;
    }
    public int GetCoins()
    {
        return _coins;
    }
    public void SetToken(string token)
    {
        _token = token;
    }
    public void SetEmail(string email)
    {
        _email = email;
    }
    public void SetNickname(string nickname)
    {
        _nickname = nickname;
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
    public void SetFreeNickNameToFalse()
    {
        _hasFreeNickName = false;
    }
    public void SetFreeNickNameToTrue()
    {
        _hasFreeNickName = true;
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


        gameData.Token = _token;
        gameData.Email = _email;
        gameData.Nickname = _nickname;
        gameData.HasFreeNickName = _hasFreeNickName;
    }

    public void ToggleMusic()
    {
        //TODO
    }
    public void ToggleSound()
    {
        //TODO

    }
}