using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public const int MAX_LIVES = 3;
    public static GameManager Instance { get; private set; }//RENAME

    [SerializeField]
    private int _coins;
    [SerializeField]
    private int _lives;

    // Start is called before the first frame update

    // public static GameManager Instance
    // {
    //     get
    //     {
    //         if (_instance is null)
    //         {
    //             Debug.LogWarning("GAME MANAGER IS NULL");
    //         }
    //         else
    //         {
    //             Debug.LogWarning("GAME MANAGER IS INSTANTIATED");
    //         }
    //         return _instance;
    //     }
    // }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
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



}
