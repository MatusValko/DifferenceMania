using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // public string Name;
    public int Coins;
    public int Lives;
    public int Experience;
    // public List<int> UnlockedLevels;
    public int UnlockedLevels;
    public int SelectedPFP;
    // public int UnlockedPFP;
    public List<int> UnlockedPFP;

    //LOGIN/REGISTER DATA
    public string Token;
    public string Nickname;
    public string Email;
    public string Device_name;
    // public string Password;
    public bool FreeNickName;
    public GameData()
    {
        Coins = 50;
        Lives = GameManager.MAX_LIVES;
        UnlockedLevels = 1;
        UnlockedPFP = new List<int>
        {
            1
        };
        SelectedPFP = 1;
        Experience = 0;


        Device_name = null;
        Email = null;
        Token = null;
        Nickname = null;
        FreeNickName = true;
        // UnlockedLevels = new List<int>();

    }

}
