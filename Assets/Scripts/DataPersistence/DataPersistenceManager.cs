using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    private GameData _gameData;

    private FileDataHandler dataHandler;
    public static DataPersistenceManager Instance { get; private set; }

    // public GameManager gameManageris;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("Found more than one Persistence Manager in the scene");
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

        LoadGame();
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
            Debug.LogError("No data was found. Initializing data to defaults");
            NewGame();
            //NEW USER
        }

        GameManager.Instance.LoadData(_gameData);

        Debug.Log("Loaded coins = " + _gameData.Coins);
        Debug.Log("Loaded lives = " + _gameData.Lives);

    }

    public void SaveGame()
    {
        if (_gameData == null)
        {
            Debug.LogError("No data was found. Initializing data to defaults, KAPPA");
            // NewGame();
            // GameManager.Instance.LoadData(_gameData);
        }

        GameManager.Instance.SaveData(ref _gameData);

        Debug.Log("Saved coins = " + _gameData.Coins);
        Debug.Log("Saved lives = " + _gameData.Lives);


        dataHandler.Save(_gameData);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
