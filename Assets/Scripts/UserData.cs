using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserDataResponse
{
    public string status;
    public string message;
    public UserData data;
}

[Serializable]
public class UserData
{
    public string name;
    public string email;
    public string nickname;
    public int stars_collected;
    public int finished_levels;
    public int level;
    public int experience;
    public int coins;
    public int lives;
    public bool has_ads_removed;
    public int last_refill_timestamp;
    public bool free_nickname_available;
    public bool rewarded_for_acc_connection;
    public int max_lives;
    public int? life_refill_time;
    public int boost_bonus_time;
    public int boost_hint;
    public int experience_to_next_level;
    public List<DailyReward> dailyRewards;
}

[Serializable]
public class DailyReward
{
    public int day;
    public int reward;
    public bool opened;
}

[Serializable]
public class LevelLostResponse
{
    public string message;
    public int lives;
}
