using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    [SerializeField] private GameData _gameData;

    [SerializeField] private FileDataHandler dataHandler;
    public static DataPersistenceManager Instance { get; private set; }

    // public GameManager gameManageris;

    public bool DATA_HANDLER_NULL = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            DebugLogger.LogWarning("Found more than one Persistence Manager in the scene");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }
    void Start()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        // gameManageris = GameManager.Instance;

        _test();
    }


    [Conditional("UNITY_EDITOR")]
    private void _test()
    {
        LoadGame(); //HANDLES LevelLoader.cs when starting the game //DELETE AFTER TESTING
    }
    [Conditional("UNITY_EDITOR")]
    void Update()
    {
        if (dataHandler == null)
        {
            DATA_HANDLER_NULL = true;
        }
    }



    public void NewGame()
    {
        _gameData = new GameData();

    }

    public void LoadGame()
    {
        _gameData = dataHandler.Load();

        if (_gameData == null)
        {
            DebugLogger.LogError("No data was found. Initializing data to defaults");
            NewGame();
            //NEW USER
        }

        GameManager.Instance.LoadData(_gameData);

        DebugLogger.Log("Loaded coins = " + _gameData.Coins);
        DebugLogger.Log("Loaded lives = " + _gameData.Lives);

    }

    public void SaveGame()
    {
        if (_gameData == null)
        {
            DebugLogger.LogError("No data was found. Initializing data to defaults, KAPPA");
            // NewGame();
            // GameManager.Instance.LoadData(_gameData);
        }

        GameManager.Instance.SaveData(ref _gameData);

        DebugLogger.Log("Saved coins = " + _gameData.Coins);
        DebugLogger.Log("Saved lives = " + _gameData.Lives);


        if (dataHandler == null)
        {
            DebugLogger.LogError("No dataHANDLER was found");
        }

        dataHandler.Save(_gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
