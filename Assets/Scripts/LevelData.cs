using System.Collections.Generic;

[System.Serializable]
public class LevelData
{
    public int id;
    public int episode_id;
    public int name;
    public int reward_coins;
    public int stars_collected;
}

[System.Serializable]
public class EpisodeData
{
    public int id;
    public int unlock_stars;
    public int unlock_coins;
    public List<LevelData> levels;
}

[System.Serializable]
public class EpisodeListData
{
    public List<EpisodeData> episodes;
}