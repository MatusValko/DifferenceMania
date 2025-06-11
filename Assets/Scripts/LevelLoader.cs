using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour
{

    // public GameObject loadingScreen;
    [SerializeField]
    private Slider _leftSlider;
    [SerializeField]
    private Slider _rightSlider;
    [SerializeField]
    private TextMeshProUGUI _loadingText;
    [SerializeField]
    private string _errorText = "ERROR";


    [SerializeField]
    private bool SERVER_ERROR = false;


    [Header("SharRise")]
    [SerializeField] private GameObject _sharRiseCanvas;
    [SerializeField] private GameObject _diffManiaCanvas;
    [SerializeField] private GameObject _loadingCanvas;
    // [SerializeField] private bool isIntro = true;


    public void LoadGame()
    {
        DebugLogger.Log("Loading Game");
        //MAIN MENU - 1
        //LOGIN - 2
        StartCoroutine(LoadAsynchronously(1));
    }

    IEnumerator IntroLogo()
    {
        //play music
        SoundManager.PlaySound(SoundType.LOGIN_THEME, 1, 0);
        yield return new WaitForSeconds(1);
        SoundManager.PlaySound(SoundType.LOGIN_THEME, 3, 1);
        yield return new WaitForSeconds(1);

        _sharRiseCanvas.SetActive(false);
        _diffManiaCanvas.SetActive(true);
        _loadingCanvas.SetActive(true);
        SoundManager.PlayThemeSound(SoundType.MAIN_MENU_THEME);
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        yield return StartCoroutine(CheckServerConnection());

        float progress = 0f;
        //CHECK INTERNET CONNECTION
        int MAX = 1000;
        for (int i = 0; i < MAX; i++)
        {
            progress = (float)i / MAX;
            ChangeLoading(progress);
            // Debug.Log(value);
            if (progress >= 0.2)
            {
                progress = 0.2f;
                break;
            }
            yield return null; // Pause the coroutine until the next frame
        }

        if (SERVER_ERROR)
        {
            _loadingText.text = _errorText;
            yield break;
        }

        //IF CONNECTED TO SERVER THEN LOAD SAVED DATA
        //IF ACCOUNT IS LOGGED IN THEN GET GAMED DATA FROM SERVER




        //TESTING
        yield return new WaitForSeconds(1);
        //TESTING MAYBE UNCOMMENT IF ERROR IS NOT SHOWING
        // MAX = 1000;
        // for (int i = 0; i < MAX; i++)
        // {
        //     if (SERVER_ERROR)
        //     {
        //         yield break;
        //     }
        //     // slider.value =  i % MAX;
        //     float value = (float)i / MAX;
        //     ChangeLoading(value);
        //     // Debug.Log(value);
        //     yield return null; // Pause the coroutine until the next frame
        // }


        //isIntro = false; TODO 
        //LOAD LEVEL 
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        // operation.allowSceneActivation = false;
        float oldProgress = progress;

        while (!operation.isDone)
        {
            float value = Mathf.Clamp01(operation.progress / .9f);
            value += oldProgress;
            value = value >= 1 ? 1 : value;

            progress = value;
            // Debug.Log(progress);
            ChangeLoading(progress);
            yield return null;
        }



    }
    public IEnumerator CheckServerConnection()
    {
        UnityWebRequest request = UnityWebRequest.Get(GameManager.GAMESERVER);
        using (request)
        {
            request.timeout = 5;
            yield return request.SendWebRequest();

            // NO CONNECTION TO SERVER, CHECK IF INTERNET IS AVAIBLE
            if (request.error != null)
            {
                _errorText = "Can't connect to the game server, please wait.";
                DebugLogger.LogError(_errorText);
                SERVER_ERROR = true;
                yield return StartCoroutine(CheckInternetConnection());

            }
            else
            {
                Debug.Log("Successfully connected to game server");
                //LOAD DATA FROM SERVER OR LOCAL STORAGE

                DataPersistenceManager.Instance.LoadGame();
                GameManager.Instance.CheckIfIsLoggedIn();
                if (GameManager.Instance.ISLOGGEDIN)
                {
                    //LOAD DATA FROM SERVER
                    DebugLogger.Log("Loading data from server");
                    StartCoroutine(LoadUserData());
                    StartCoroutine(GetProgressData());
                    // GameManager.Instance.LoadDataFromServer();
                }
                else
                {
                    //LOAD DATA FROM LOCAL STORAGE
                    DebugLogger.Log("User is playing for the first time!");
                    //start login scene
                    SceneManager.LoadScene(2);

                    // GameManager.Instance.LoadDataFromLocalStorage();
                }
            }
        }
    }
    public IEnumerator CheckInternetConnection()
    {
        const string echoServer = "https://www.google.com";
        using (UnityWebRequest request = UnityWebRequest.Get(echoServer))
        {
            request.timeout = 5;
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                _errorText = "No internet connection";
                DebugLogger.LogError(_errorText);
            }
            else
            {
                DebugLogger.Log("Internet connection is available");
            }
        }
    }

    public static IEnumerator GetProgressData()
    {
        UnityWebRequest request = UnityWebRequest.Get(GameManager.API_GET_USER_LEVEL_DATA);
        DebugLogger.Log($"Loading Levels from {GameManager.API_GET_USER_LEVEL_DATA}");
        request.SetRequestHeader("Authorization", "Bearer " + GameManager.Instance.GetToken());
        request.SetRequestHeader("Accept", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {


            string jsonResponse = request.downloadHandler.text;
            List<EpisodeData> episodes = JsonConvert.DeserializeObject<List<EpisodeData>>(jsonResponse);
            GameManager.Instance.SetEpisodes(episodes);
        }
        else
        {
            Debug.LogError($"Error: {request.error}");
        }
    }

    public IEnumerator LoadUserData()
    {
        UnityWebRequest request = UnityWebRequest.Get(GameManager.API_LOAD_USER_DATA);
        request.SetRequestHeader("Authorization", "Bearer " + GameManager.Instance.GetToken());
        request.SetRequestHeader("Accept", "application/json");

        yield return request.SendWebRequest();
        // GameManager.Instance.LoadDataFromServer();
        // GameManager.Instance.LoadDataFromLocalStorage();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            DebugLogger.Log("Raw JSON: " + json);

            // Deserialize JSON response
            UserDataResponse userDataResponse = JsonConvert.DeserializeObject<UserDataResponse>(json);

            if (userDataResponse != null && userDataResponse.status == "success")
            {
                DebugLogger.Log("Device Name: " + userDataResponse.data.name);
                GameManager.Instance.SetDeviceName(userDataResponse.data.name);
                DebugLogger.Log("Email: " + userDataResponse.data.email);
                GameManager.Instance.SetEmail(userDataResponse.data.email);
                DebugLogger.Log("Nickname: " + userDataResponse.data.nickname);
                GameManager.Instance.SetNickname(userDataResponse.data.nickname);
                DebugLogger.Log("Nickname: " + userDataResponse.data.nickname);

                //TODO GET FROM SERVER
                List<int> list = new List<int> { 1, 2, 5 };
                DebugLogger.Log("Unlocked PFP: " + list);
                GameManager.Instance.SetUnlockedPFP(list);
                DebugLogger.Log("Selected PFP: " + 1);
                GameManager.Instance.SetSelectedPFP(1);

                DebugLogger.Log("Stars Collected: " + userDataResponse.data.stars_collected);
                GameManager.Instance.SetStarsCollected(userDataResponse.data.stars_collected);
                DebugLogger.Log("Finished Levels: " + userDataResponse.data.finished_levels);
                GameManager.Instance.SetFinishedLevels(userDataResponse.data.finished_levels);
                DebugLogger.Log("Experience: " + userDataResponse.data.experience);
                GameManager.Instance.SetExperience(userDataResponse.data.experience);
                DebugLogger.Log("Experience to Next Level: " + userDataResponse.data.experience_to_next_level);
                GameManager.Instance.SetExperienceToNextLevel(userDataResponse.data.experience_to_next_level);
                DebugLogger.Log("Has Ads Removed: " + userDataResponse.data.has_ads_removed);
                GameManager.Instance.SetHasAdsRemoved(userDataResponse.data.has_ads_removed);
                DebugLogger.Log("Last Refill Timestamp: " + userDataResponse.data.last_refill_timestamp);
                GameManager.Instance.SetLastRefillTimestamp(userDataResponse.data.last_refill_timestamp);
                DebugLogger.Log("Free Nickname Available: " + userDataResponse.data.free_nickname_available);
                GameManager.Instance.SetFreeNickNameAvailable(userDataResponse.data.free_nickname_available);
                DebugLogger.Log("Rewarded for Account Connection: " + userDataResponse.data.rewarded_for_acc_connection);
                GameManager.Instance.SetRewardedForAccountConnection(userDataResponse.data.rewarded_for_acc_connection);
                DebugLogger.Log("Max Lives: " + userDataResponse.data.max_lives);
                GameManager.Instance.SetMaxLives(userDataResponse.data.max_lives);
                DebugLogger.Log("Life Refill Time: " + userDataResponse.data.life_refill_time);
                GameManager.Instance.SetLifeRefillTime(userDataResponse.data.life_refill_time);
                DebugLogger.Log("Boost Bonus Time: " + userDataResponse.data.boost_bonus_time);
                GameManager.Instance.SetBoostAddTime(userDataResponse.data.boost_bonus_time);
                DebugLogger.Log("Boost Hint: " + userDataResponse.data.boost_hint);
                GameManager.Instance.SetBoostHint(userDataResponse.data.boost_hint);
                // DebugLogger.Log("Unlocked Levels: " + userDataResponse.data.finished_levels);
                // GameManager.Instance.SetUnlockedLevels(userDataResponse.data.finished_levels);
                // DebugLogger.Log("Current Wins: " + userDataResponse.data.finished_levels);
                // GameManager.Instance.SetCurrentWins(userDataResponse.data.finished_levels);
                // DebugLogger.Log("Selected PFP: " + userDataResponse.data.finished_levels);
                // GameManager.Instance.SetSelectedPFP(userDataResponse.data.finished_levels);
                // DebugLogger.Log("Unlocked PFP: " + userDataResponse.data.finished_levels);
                // GameManager.Instance.SetUnlockedPFP(userDataResponse.data.finished_levels);
                // DebugLogger.Log("Boost Hint Count: " + userDataResponse.data.finished_levels);
                // GameManager.Instance.SetBoostHintCount(userDataResponse.data.finished_levels);
                // DebugLogger.Log("Stars Collected: " + userDataResponse.data.stars_collected);
                // GameManager.Instance.SetStarsCollected(userDataResponse.data.stars_collected);
                // DebugLogger.Log("Finished Levels: " + userDataResponse.data.finished_levels);
                // GameManager.Instance.SetFinishedLevels(userDataResponse.data.finished_levels);
                // DebugLogger.Log("Experience: " + userDataResponse.data.experience);
                // GameManager.Instance.SetExperience(userDataResponse.data.experience);
                // DebugLogger.Log("Experience to Next Level: " + userDataResponse.data.experience_to_next_level);
                // GameManager.Instance.SetExperienceToNextLevel(userDataResponse.data.experience_to_next_level);
                // DebugLogger.Log("Has Ads Removed: " + userDataResponse.data.has_ads_removed);
                // GameManager.Instance.SetHasAdsRemoved(userDataResponse.data.has_ads_removed);
                // DebugLogger.Log("Last Refill Timestamp: " + userDataResponse.data.last_refill_timestamp);
                // GameManager.Instance.SetLastRefillTimestamp(userDataResponse.data.last_refill_timestamp);
                // DebugLogger.Log("Free Nickname Available: " + userDataResponse.data.free_nickname_available);
                // GameManager.Instance.SetFreeNicknameAvailable(userDataResponse.data.free_nickname_available);
                // DebugLogger.Log("Rewarded for Account Connection: " + userDataResponse.data.rewarded_for_acc_connection);
                // GameManager.Instance.SetRewardedForAccConnection(userDataResponse.data.rewarded_for_acc_connection);
                // DebugLogger.Log("Max Lives: " + userDataResponse.data.max_lives);
                // GameManager.Instance.SetMaxLives(userDataResponse.data.max_lives);

                DebugLogger.Log("User Level: " + userDataResponse.data.level);
                GameManager.Instance.SetProfileLevel(userDataResponse.data.level);
                DebugLogger.Log("Coins: " + userDataResponse.data.coins);
                GameManager.Instance.SetCoins(userDataResponse.data.coins);
                DebugLogger.Log("Lives: " + userDataResponse.data.lives);
                GameManager.Instance.SetLives(userDataResponse.data.lives);


                // Loop through daily rewards
                foreach (var reward in userDataResponse.data.dailyRewards)
                {
                    DebugLogger.Log($"Day {reward.day}: Reward {reward.reward}, Opened: {reward.opened}");
                    // You can also set these values in your GameManager if needed
                    // GameManager.Instance.SetDailyReward(reward.day, reward.reward, reward.opened);
                }
            }
            else
            {
                DebugLogger.LogError("Failed to parse user data.");
            }
        }
        else
        {
            DebugLogger.LogError("Error fetching user data: " + request.error);
        }

    }

    private void ChangeLoading(float value)
    {
        if (_leftSlider.gameObject.activeSelf == false)
        {
            return;
        }
        _leftSlider.value = value;
        _rightSlider.value = value;

        _loadingText.text = "Loading " + Math.Round(value * 100) + "%";
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IntroLogo());
        LoadGame();

    }

    // Update is called once per frame
    void Update()
    {

    }
}
