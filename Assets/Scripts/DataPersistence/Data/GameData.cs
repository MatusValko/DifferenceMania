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
    public int CurrentWins;
    public int UnlockedLevels;
    public int SelectedPFP;
    // public int UnlockedPFP;
    public List<int> UnlockedPFP;

    //LOGIN/REGISTER DATA
    public string Token;
    public string PlayerID;
    public string Nickname;
    public string Email;
    public string Device_name;
    // public string Password;
    public bool HasFreeNickName;
    public GameData()
    {
        Coins = 50;
        Lives = 5;
        UnlockedLevels = 1;
        UnlockedPFP = new List<int>
        {
            1
        };
        SelectedPFP = 1;
        Experience = 0;
        CurrentWins = 0;

        Device_name = null;
        Email = null;
        Token = null;
        PlayerID = null;
        Nickname = null;
        HasFreeNickName = true;
        // UnlockedLevels = new List<int>();

    }

}
