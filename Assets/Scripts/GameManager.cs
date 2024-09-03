using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour, IDataPersistenceManager
{
    public const int MAX_LIVES = 3;

    public bool ISLOGGEDIN = false;
    public static GameManager Instance { get; private set; }//RENAME

    [Header("Currencies")]
    [SerializeField]
    private int _coins;
    [SerializeField]
    private int _lives;

    [Header("Account details")]
    [SerializeField]
    private string _token;
    [SerializeField]
    private string _nickname;
    [SerializeField]
    private string _email;
    [SerializeField]
    private string _device_name;
    [SerializeField]
    private string _password;
    [SerializeField]
    private string _experience;
    [SerializeField]
    private int _unlockedLevels;
    [SerializeField]
    private int _selectedPFP;
    [SerializeField]
    private int _unlockedPFP;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            Debug.LogWarning("GAME MANAGER IS INSTANTIATED");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
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

    public void LoadData(GameData gameData)
    {
        _coins = gameData.Coins;
        _lives = gameData.Lives;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.Coins = _coins;
        gameData.Lives = _lives;

    }

}
