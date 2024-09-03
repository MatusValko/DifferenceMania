using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public string Name;
    public int Coins;
    public int Lives;



    public GameData()
    {
        Coins = 50;
        Lives = GameManager.MAX_LIVES;

    }

}
