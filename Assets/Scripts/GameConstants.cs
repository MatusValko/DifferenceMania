public static class GameConstants
{
    // public const string GAMESERVER = "http://localhost";
    public const string GAMESERVER = "https://diff.nconnect.sk";

    public const string API_REGISTER = GAMESERVER + "/api/register";
    public const string API_LOGIN = GAMESERVER + "/api/login";
    public const string API_DIFF_IMAGES = GAMESERVER + "/api/diff-iamges";
    public const string API_LOAD_USER_DATA = GAMESERVER + "/api/load_user_data";
    public const string API_GET_USER_LEVEL_DATA = GAMESERVER + "/api/progress";
    public const string API_ADD_EMAIL = GAMESERVER + "/api/add-email";


    public static string API_GET_LEVEL_WIN(int levelId)
    {
        return GAMESERVER + $"/api/level/{levelId}/win";
    }

    public static string API_GET_LEVEL_LOSS(int levelId)
    {
        return GAMESERVER + $"/api/level/{levelId}/loss";
    }
}