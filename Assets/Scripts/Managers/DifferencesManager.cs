using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DifferencesManager : MonoBehaviour
{
    public static DifferencesManager Instance { get; private set; }

    [SerializeField] private Camera _mainCamera;


    [SerializeField]
    private List<Difference> _differences;

    [SerializeField]
    private int _differencesCount = 0;
    [SerializeField]
    private GameObject[] _bottomCircles;

    [SerializeField]
    private int _foundDifferences = 0;



    [SerializeField]
    private bool _isHurryUp = false;
    [SerializeField] private bool _isLoaded = false;
    [SerializeField] private int _downloadedImages = 0;


    [SerializeField] public int GotStarsFromLevel = -1; // Number of stars collected

    [Serializable]
    public class DifferenceJSON
    {
        public float x;
        public float y;
        public float width;
        public float height;
    }

    // [Serializable]
    // private class DifferencesData
    // {
    //     public string image_id;
    //     public List<DifferenceJSON> differences;
    // }

    [Serializable]
    public class ImageData
    {
        public string name;
        public string path;
        public int differences;
        public int difficulty;
        [JsonProperty("json_diff")]
        public List<DifferenceJSON> jsonDiff;
        public string created_at;
        public string updated_at;
    }

    [Serializable]
    public class LevelDataGame
    {
        public int total_time_limit;
        [JsonProperty("1_star")]
        public int oneStar;
        [JsonProperty("2_star")]
        public int twoStar;
        [JsonProperty("3_star")]
        public int threeStar;
        public int bonus_total_time_limit;
        [JsonProperty("1_star_bonus_time")]
        public int oneStarBonusTime;
        [JsonProperty("2_star_bonus_time")]
        public int twoStarBonusTime;
        [JsonProperty("3_star_bonus_time")]
        public int threeStarBonusTime;
        public List<ImageData> images;
    }

    [SerializeField] private LevelDataGame _levelData;

    [SerializeField] private GameObject _firstImage;
    [SerializeField] private GameObject _secondImage;

    [SerializeField] private GameObject _foundCircle;

    [SerializeField] private float zoomSpeed = 0.0001f;

    [SerializeField] private float maxZoom = 8;
    [SerializeField] private float minZoom = 1;

    // [SerializeField] private float _timePerDifference = 20f; // Time allocated per difference (in seconds)




    [SerializeField] private float _totalTime; // Total countdown time
    [SerializeField] private bool _isPaused = false;
    private bool _isGameOver = false;
    [SerializeField] private GameObject _xImage; // Prefab for the "X" image

    [SerializeField] private GameObject _levelCompleted;
    [SerializeField] private GameObject _outOfTime;

    [SerializeField] private GameObject _congratulationWindow;
    [SerializeField] private GameObject _youLoseWindow;
    [SerializeField] private GameObject _youLosePopUpWindow;
    [SerializeField] private GameObject _quitWindow;



    [Header("Testing")]
    [SerializeField] private TextMeshProUGUI _levelIDText; // UI Text to display the level ID




    [Header("Top bar UI")]
    [SerializeField] Animator _topBarUIAnimator;
    [SerializeField] private TextMeshProUGUI _timerText; // UI Text to display the timer
    [SerializeField] private TextMeshProUGUI _lifeText;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private TextMeshProUGUI _levelText; // UI Text to display the level number

    [Header("Boosts")]
    //private variable for boost hint and boost add time text
    [SerializeField] private TextMeshProUGUI _boostHintTextAmount;
    [SerializeField] private TextMeshProUGUI _boostAddTimeTextAmount;
    //gameobject for boost hint and boost add time
    // [SerializeField] private GameObject _boostHint;
    // [SerializeField] private GameObject _boostAddTime;
    //gameobject for ribbons
    [SerializeField] private GameObject _ribbonHint;
    [SerializeField] private GameObject _ribbonAddTime;

    [SerializeField] private GameObject _buyHintWindow;
    [SerializeField] private GameObject _buyAddTimeWindow;

    [Header("Error Winodw")]
    [SerializeField] private GameObject _errorWindow;
    [SerializeField] private TextMeshProUGUI _errorText;




    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            // DebugLogger.Log("DIFFERENCE MANAGER IS INSTANTIATED");
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(Instance);
        }
    }
    void Start()
    {
        Initialisation();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Initialisation();
    }
    private void Initialisation()
    {
        SoundManager.PlayThemeSound(SoundType.GAME_THEME, 0.7f);
        StartCoroutine(SetUpGame(GameManager.Instance.GetLevelID()));

    }

    private void _setUpTimeAndDifferences()
    {
        // Set up the time and differences based on the level data
        _totalTime = _levelData.total_time_limit;
        _differencesCount = _levelData.images[0].differences;
        DebugLogger.Log($"Total differences: {_differencesCount}");


        int index = 0;
        foreach (var diff in _levelData.images[0].jsonDiff)
        {
            Difference difference1 = Difference.CreateDifference(diff.x, diff.y, diff.width, diff.height, index, _firstImage);
            _differences.Add(difference1);
            Difference difference2 = Difference.CreateDifference(diff.x, diff.y, diff.width, diff.height, index, _secondImage);
            _differences.Add(difference2);
            // _differencesCount++;
            index++;
        }

        // Load differences from JSON file
    }

    //get game data
    public LevelDataGame GetLevelData()
    {
        return _levelData;
    }
    //get time, convert to int
    public int GetTime()
    {
        return (int)_totalTime;
    }

    IEnumerator SetUpGame(int levelId)
    {
        //for zooming on mobile
        targetScale = transform.localScale;

        string url = GameManager.Instance.GetLevelDataURL(levelId);

        UnityWebRequest request = UnityWebRequest.Get(url);
        string token = GameManager.Instance.GetToken();
        if (string.IsNullOrEmpty(token))
        {
            // #if UNITY_EDITOR
            //             token = "Ay0p5La74VhxJVcjCOA2K1YRWUYZ4ooumTkNs5lN49ca3267";
            //             DebugLogger.LogWarning($"Setting up default token for testing in Unity Editor. Token: {token}");
            // #endif

            if (string.IsNullOrEmpty(token))
            {
                DebugLogger.LogWarning("Token is null or empty.");
                _errorWindow.SetActive(true);
                _errorText.text = "Token is null or empty.";
                yield break; // Exit the coroutine if token is not set
            }
        }
        //if in unity editor, set token to empty string

        DebugLogger.Log($"Loading Level Data from {url} with token: {token}");
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Accept", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            DebugLogger.Log($"Level {levelId} data: {jsonResponse}");
            _levelData = JsonConvert.DeserializeObject<LevelDataGame>(jsonResponse);

            //TESTING
            _levelIDText.text = $"Level ID: {_levelData.images[0].path}"; // Set the level ID text

            StartCoroutine(DownloadImage(_levelData.images[0].path, 1)); // Load first image
            StartCoroutine(DownloadImage(_levelData.images[0].path, 2)); // Load first image

            _setUpTimeAndDifferences();
            // SHOW FOUND CIRCLES EQUAL TO NUMBER OF DIFFERENCES
            _adjustCircles();
            //GET TOTAL TIME TO SOLVE
            _updateTimerUI();
            _adjustBoosts();
            _adjustTopBarUI();
            // _isLoaded = true;
        }
        else
        {
            DebugLogger.LogError($"Error: {request.error}");
#if  !UNITY_EDITOR
            _errorWindow.SetActive(true);
            _errorText.text = "Error loading or parsing JSON data: " + request.error;
#endif
            // Parse the response and update the level data accordingly
        }
    }

    IEnumerator DownloadImage(string url, int imageIndex)
    {

        string urlImage = GameConstants.GAMESERVER + $"/{url}/{imageIndex}.jpg";

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(urlImage))
        {
            request.SetRequestHeader("Authorization", "Bearer " + GameManager.Instance.GetToken());
            request.SetRequestHeader("Accept", "application/json");
            yield return request.SendWebRequest(); // Wait for response

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = SpriteFromTexture(texture); // Convert and assign
                if (imageIndex == 1)
                {
                    _firstImage.GetComponent<Image>().sprite = sprite;
                    _downloadedImages++;
                }
                else if (imageIndex == 2)
                {
                    _secondImage.GetComponent<Image>().sprite = sprite;
                    _downloadedImages++;
                    //pause in unity editor

                }
                else
                {
                    DebugLogger.LogError("Invalid image index: " + imageIndex);
                    _errorWindow.SetActive(true);
                    _errorText.text = "Error loading image: " + request.error;

                }
            }
            else
            {
                DebugLogger.LogError("Failed to load image: " + request.error);

                //show error window
                _errorWindow.SetActive(true);
                _errorText.text = "Error loading or parsing JSON data: " + request.error;


            }


        }
        StartPlaying();
    }

    //Start level
    public void StartPlaying()
    {
        if (_downloadedImages >= 2)
        {
            _isLoaded = true;
            GameManager.Instance.FadeInLevel();
        }
    }
    Sprite SpriteFromTexture(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    void Update()
    {
        if (_isLoaded == false) return;
        if (_isGameOver) return;
        if (_isPaused) return;

        _timeTick();

        // Detect background clicks
        _mobileInput();
        // _click();
    }

    private void _click(Vector2 mousePosition)
    {
        if (_isPaused || !_isLoaded)
        {
            return;
        }
        // if (Input.GetMouseButtonDown(0))
        // {
        // Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

        // Clicked outside of a difference
        if (hitCollider == null)
        {
            DebugLogger.Log("CLICKED OUTSIDE!");
            return;
        }

        if (hitCollider.gameObject.CompareTag("Difference"))
        {
            Difference difference = hitCollider.gameObject.GetComponent<Difference>();  // Get the Difference component of the clicked object
            if (difference != null)
            {
                // DebugLogger.Log("CLICKED ON DIFFERENCE!");
                Clicked(difference.id);
                //play correct click sound
                SoundManager.PlaySound(SoundType.GAME_CORRECT_CLICK);

            }
        }
        else if (hitCollider.gameObject.CompareTag("Image"))
        {
            // DebugLogger.LogWarning("NO DIFFERENCE CLICKED!");
            _takeTime();

            // Instantiate the "X" image at the clicked position
            Vector3 position = new Vector3(mousePosition.x, mousePosition.y, 0);
            // Quaternion rotation = Quaternion.identity; //OLD
            Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
            if (hitCollider.gameObject.name == "Image1")
            {
                Instantiate(_xImage, position, rotation, _firstImage.transform);
            }
            else
            {
                Instantiate(_xImage, position, rotation, _secondImage.transform);
            }
            //play incorrect click sound with 50% lower volume
            SoundManager.PlaySound(SoundType.GAME_INCORRECT_CLICK, volume: 0.6f);
            //TODO vibrate the phone

        }
        // }
    }

    private Vector2 _startPos;
    private float _startTime;
    private bool _isDragging = false;
    private Vector3 targetScale;

    [SerializeField] private float clickTimeThreshold = 0.2f; // seconds
    [SerializeField] private float moveThreshold = 10f; // pixels
    public float smoothSpeed = 30f; // Higher = faster zooming, lower = smoother
    private Vector3 ClampScale(Vector3 scale, float min, float max)
    {
        float clampedX = Mathf.Clamp(scale.x, min, max);
        float clampedY = Mathf.Clamp(scale.y, min, max);
        return new Vector3(clampedX, clampedY, 1f);
    }
    private Vector2 NormalizedPivot(Vector2 localPos, RectTransform rect)
    {
        return new Vector2(
            (localPos.x + rect.rect.width * 0.5f) / rect.rect.width,
            (localPos.y + rect.rect.height * 0.5f) / rect.rect.height
        );
    }

    private void _mobileInput()
    {
        // Handle mobile touch
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _startPos = touch.position;
                    _startTime = Time.time;
                    _isDragging = false;
                    break;

                case TouchPhase.Moved:
                    if (Vector2.Distance(touch.position, _startPos) > moveThreshold)
                        _isDragging = true;
                    break;

                case TouchPhase.Ended:
                    float duration = Time.time - _startTime;
                    if (!_isDragging && duration < clickTimeThreshold)
                    {
                        Vector2 worldPos = _mainCamera.ScreenToWorldPoint(touch.position);
                        _click(worldPos);
                    }
                    break;
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
            float deltaMagnitudeDiff = currentMagnitude - prevMagnitude;
            // Calculate new scale based on pinch difference
            // float scaleFactor = 1 + (difference * zoomSpeed);
            // scaleFactor = Mathf.Clamp(scaleFactor, 0.8f, 1.2f); // Prevent extreme scaling per frame

            float scaleFactor = deltaMagnitudeDiff * zoomSpeed;
            Vector3 newTargetScale = targetScale + Vector3.one * scaleFactor;
            float clamped = Mathf.Clamp(newTargetScale.x, minZoom, maxZoom);
            newTargetScale = new Vector3(clamped, clamped, 1f);
            Vector2 screenMid = (touchZero.position + touchOne.position) / 2f;

            Vector2 localPoint;
            RectTransform rectTransform = _firstImage.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                DebugLogger.LogError("DID NOT FOUND RECT TRANSFORM ON FIRST IMAGE");
                return;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform, screenMid, null, out localPoint
            );
            // Calculate pivot shift
            Vector2 pivotDelta = (Vector2)rectTransform.pivot - NormalizedPivot(localPoint, rectTransform);
            rectTransform.anchoredPosition += new Vector2(
              pivotDelta.x * rectTransform.rect.width * (newTargetScale.x - rectTransform.localScale.x),
              pivotDelta.y * rectTransform.rect.height * (newTargetScale.y - rectTransform.localScale.y)
          );
            targetScale = newTargetScale;


            // Clamp the scale to minZoom and maxZoom
            // targetScale += Vector3.one * scaleFactor;
            // targetScale = ClampScale(targetScale, minZoom, maxZoom);

            // newScale = ClampScale(newScale, minZoom, maxZoom);

            // _firstImage.transform.localScale = newScale;
            // _secondImage.transform.localScale = newScale;

            // Optional: handle zooming logic here if you want
        }

        // Handle mouse input (desktop/web)
        else if (Input.GetMouseButtonDown(0))
        {
            DebugLogger.Log("BUTTON DOWN");
            _startPos = Input.mousePosition;
            _startTime = Time.time;
            _isDragging = false;
        }
        else if (Input.GetMouseButton(0))
        {
            if (Vector2.Distance((Vector2)Input.mousePosition, _startPos) > moveThreshold)
            {
                _isDragging = true;
                DebugLogger.Log("DRAGGING");
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            DebugLogger.Log("BUTTON UP");

            float duration = Time.time - _startTime;
            if (!_isDragging && duration < clickTimeThreshold)
            {
                DebugLogger.Log("CLICK");
                Vector2 worldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                _click(worldPos);
            }
        }
#if !UNITY_EDITOR
        _firstImage.transform.localScale = Vector3.Lerp(_firstImage.transform.localScale, targetScale, Time.deltaTime * smoothSpeed);
        _secondImage.transform.localScale = Vector3.Lerp(_secondImage.transform.localScale, targetScale, Time.deltaTime * smoothSpeed);
#endif
    }

    private void _timeTick()
    {
        // Countdown logic
        _totalTime -= Time.deltaTime;

        // Update the timer UI
        _updateTimerUI();

        //if less than 10 seconds, play time animation 
        if (_totalTime <= 10 && !_isHurryUp)
        {
            //get parent gameobject of timer text and play animation
            _topBarUIAnimator.SetTrigger("HurryUp");
            DebugLogger.LogWarning("HURRY UP!");
            _isHurryUp = true;
            SoundManager.PlayAudioClip(SoundType.GAME_TIME);
        }
        else if (_totalTime > 10 && _isHurryUp)
        {
            _topBarUIAnimator.SetTrigger("EndHurryUp");
            _isHurryUp = false;
            SoundManager.StopClip();
        }

        // Check for game over
        if (_totalTime <= 0)
        {
            DebugLogger.Log("Run out of time!");
            _gameOver();
        }
    }

    public void OpenQuitLevelWindow()
    {
        if (_quitWindow.activeSelf)
        {
            // If the quit window is already open, close it
            return;
        }
        _quitWindow.SetActive(true);
        _quitWindow.GetComponent<Animator>()?.Update(Time.unscaledDeltaTime);
        TogglePause();
    }

    public void TogglePause()
    {
        if (_isPaused)
        {
            _resumeGame();
        }
        else
        {
            _pauseGame();
        }
    }
    private void _resumeGame()
    {
        // Time.timeScale = 1;  // Resume time
        _isPaused = false;
        DebugLogger.Log("Game Resumed");
    }

    private void _pauseGame()
    {
        // Time.timeScale = 0;  // Pause time
        _isPaused = true;
        DebugLogger.Log("Game Paused");
    }

    private void _updateTimerUI()
    {
        // Display time in minutes and seconds
        int minutes = Mathf.FloorToInt(_totalTime / 60);
        int seconds = Mathf.FloorToInt(_totalTime % 60);
        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void RestartGame()
    {
        _totalTime = _levelData.total_time_limit;
        _isGameOver = false;
    }



    private void _adjustCircles()
    {
        for (int i = 1; i <= _bottomCircles.Length; i++)
        {
            if (i > _differencesCount)
            {
                // DebugLogger.Log("Differences: " + _differencesCount);
                _bottomCircles[i - 1].transform.parent.parent.gameObject.SetActive(false);
            }
        }
    }
    private void _adjustCorrectCircles()
    {
        _bottomCircles[_foundDifferences - 1].SetActive(true);
    }

    public void Clicked(int index)
    {
        // Debug.Log("CLICKED");
        List<Difference> foundDifferences = _differences.FindAll(Difference => Difference.id == index);
        Difference differenceLocal = null;
        GameObject circle = null;
        foreach (var difference in foundDifferences)
        {
            differenceLocal = difference;
            difference.gameObject.SetActive(false);
        }
        DebugLogger.Log("Vytvaram Image" + differenceLocal.id);
        // X = COLLIDER X + (SIZE / 2) - (WIDHT OF CIRCLE / 2)
        // Y = COLLIDER Y + (SIZE / 2) - (HEIGHT OF CIRCLE / 2)
        Vector3 position = new Vector3(differenceLocal.x + (differenceLocal.width / 2) - 70, -differenceLocal.y - (differenceLocal.height / 2) + 70, 0); // z = 0 for 2D
        //Spawn circle image na najdenom rozdiely na obi dvoch obrazkoch
        circle = Instantiate(_foundCircle, position, Quaternion.identity, _firstImage.transform);
        circle.transform.localPosition = position;
        circle = Instantiate(_foundCircle, position, Quaternion.identity, _secondImage.transform);
        circle.transform.localPosition = position;

        //FOUND ANOTHER DIFFERENCE
        _foundDifferences++;
        DebugLogger.Log("Davam fajku :)");
        _adjustCorrectCircles();

        if (_foundDifferences == _differencesCount)
        {
            _gameWon();
        }
    }

    private void _gameOver()
    {
        _topBarUIAnimator.SetTrigger("EndHurryUp");
        // _outOfTime.SetActive(true);
        _isGameOver = true;
        _timerText.text = "00:00"; // Display zero time
        _totalTime = 0;
        DebugLogger.Log("Game Over!");
        StartCoroutine(_sendGameLostToServer());
        _youLosePopUpWindow.SetActive(true);
    }
    public void YouLoseContinueButton()
    {
        SoundManager.StopClip();
        SoundManager.StopTheme();
        SoundManager.PlaySound(SoundType.GAME_LOSE);
        _youLosePopUpWindow.SetActive(false);
        _youLoseWindow.SetActive(true);
    }
    public void YouLoseWatchADtoPlayOn()
    {
        //play ad
        //if ad is watched, continue to play
        //if ad is not watched, show popup window

        _youLosePopUpWindow.SetActive(false);
        //add time
        AddTime();
        _isGameOver = false;
    }

    private void _gameWon()
    {
        DebugLogger.Log("Found all differences!, Game WON!");
        //MAYBE TEMPORALY FREEZE TIME
        // Time.timeScale = 0;
        _isGameOver = true;
        // _levelCompleted.SetActive(true);
        //play level complete sound
        SoundManager.StopClip();
        SoundManager.StopTheme();
        SoundManager.PlaySound(SoundType.GAME_WIN, 0.9f);

        //send lelvel won to server
        StartCoroutine(_sendGameWonToServer());

        _playWinAnimation();

        //start coroutine to next window, congratulations
        StartCoroutine(_showCongratulation());
    }

    //parse string like images/20 and get only number after the last slash
    private int _parseImageID(string path)
    {
        string[] parts = path.Split('/');
        if (parts.Length > 0)
        {
            string lastPart = parts[parts.Length - 1];
            if (int.TryParse(lastPart, out int imageID))
            {
                return imageID;
            }
        }
        DebugLogger.LogError("Failed to parse image ID from path: " + path);
        return -1; // Return -1 if parsing fails
    }

    IEnumerator _sendGameLostToServer()
    {
        string url = GameConstants.API_GET_LEVEL_LOSS(GameManager.Instance.GetLevelID());
        DebugLogger.Log($"Sending Game Lost to {url}");
        UnityWebRequest request = UnityWebRequest.Post(url, new WWWForm());
        request.SetRequestHeader("Authorization", "Bearer " + GameManager.Instance.GetToken());
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Content-Type", "application/json");
        // Create the JSON data
        // DebugLogger.LogError(_levelData.images[0].path);
        // var data = new
        // {
        //      stars_collected = _getLeveLStars(),
        //     score = 69, //TODO HOW TO CALCULATE SCORE?
        //     images_finished = new List<int> { _parseImageID(_levelData.images[0].path) }
        // };
        // string jsonData = JsonConvert.SerializeObject(data);
        // request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            DebugLogger.Log("Game Lost sent successfully: " + request.downloadHandler.text);
            /*{
                "message": "Level lost",
                "lives": 4
            }*/
            // Access response data
            var responseJson = request.downloadHandler.text;
            LevelLostResponse levelLostResponse = JsonConvert.DeserializeObject<LevelLostResponse>(responseJson);
            if (levelLostResponse != null)
            {
                DebugLogger.Log("Message: " + levelLostResponse.message);
                DebugLogger.Log("Lives left after loss: " + levelLostResponse.lives);
                GameManager.Instance.SetLives(levelLostResponse.lives);
            }
            else
            {
                DebugLogger.LogError("Failed to parse user data.");
            }
        }
        else
        {
            DebugLogger.LogError("Error sending Game Won data: " + request.error);
            _errorWindow.SetActive(true);
            _errorText.text = "Error sending Game Won data: " + request.error;
        }
    }

    IEnumerator _sendGameWonToServer()
    {
        string url = GameConstants.API_GET_LEVEL_WIN(GameManager.Instance.GetLevelID());
        DebugLogger.Log($"Sending Game Won to {url}");
        UnityWebRequest request = UnityWebRequest.Post(url, new WWWForm());
        request.SetRequestHeader("Authorization", "Bearer " + GameManager.Instance.GetToken());
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Content-Type", "application/json");
        // Create the JSON data
        // DebugLogger.LogError(_levelData.images[0].path);
        var data = new
        {
            stars_collected = _getLeveLStars(),
            score = 69, //TODO HOW TO CALCULATE SCORE?
            images_finished = new List<int> { _parseImageID(_levelData.images[0].path) }
        };
        string jsonData = JsonConvert.SerializeObject(data);
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            DebugLogger.Log("Game Won data sent successfully: " + request.downloadHandler.text);
            /*{
                "message": "Level finished",
                "level": {
                    "level_up": false,
                    "rewards": {
                        "coins": 0
                    }
                }
            }*/
        }
        else
        {
            DebugLogger.LogError("Error sending Game Won data: " + request.error);
            _errorWindow.SetActive(true);
            _errorText.text = "Error sending Game Won data: " + request.error;
        }
    }

    private int _getLeveLStars()
    {
        int time = GetTime();
        int spentTime = _levelData.total_time_limit - time;
        DebugLogger.Log("Total Time: " + _levelData.total_time_limit + " Time spent: " + spentTime + " 3STARS: " + _levelData.threeStar + " 2STARS: " + _levelData.twoStar + " 1STAR: " + _levelData.oneStar);

        if (spentTime <= _levelData.threeStar) //check if time is less than 0
        {
            GotStarsFromLevel = 3; // Set the number of stars collected
        }
        else if (spentTime <= _levelData.twoStar)
        {
            GotStarsFromLevel = 2; // Set the number of stars collected
        }
        else if (spentTime <= _levelData.oneStar)
        {
            GotStarsFromLevel = 1; // Set the number of stars collected
        }
        else
        {
            GotStarsFromLevel = 0; // Set the number of stars collected
        }
        return GotStarsFromLevel;
    }

    private void _playWinAnimation()
    {
        StartCoroutine(PlayWinAnimationsSequentially());
    }
    // Local coroutine to play animations with delay
    IEnumerator PlayWinAnimationsSequentially()
    {
        float totalDuration = 2f;
        int count = _bottomCircles.Length;
        float delay = totalDuration / count;

        for (int i = 0; i < count; i++)
        {
            var animator = _bottomCircles[i].transform.parent.parent.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("LevelWon");
            }
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator _showCongratulation()
    {
        yield return new WaitForSeconds(3);
        _congratulationWindow.SetActive(true);
    }

    //function to take live when player dont click on difference
    private void _takeTime()
    {
        _totalTime -= 10;
        _updateTimerUI();
    }

    //function to add 60 seconds to totaltime, onclick button
    public void AddTime()
    {
        _totalTime += 60;
        _updateTimerUI();
        _topBarUIAnimator.SetTrigger("AddTime");
    }

    //function to quit level, onclick button
    public void QuitLevel()
    {
        _isLoaded = false;
        Time.timeScale = 1;
        SoundManager.StopTheme();
        GameManager.Instance.FadeOutToMainMenu();
    }

    //function to show boosts texts if gamemanager has enough of them, otherwise show ribbon
    public void _adjustBoosts()
    {
        //if player has enough of hints, show hint boost
        if (GameManager.Instance.GetBoostHintCount() > 0)
        {
            // _boostHint.SetActive(true);
            _ribbonHint.SetActive(false);
            // Adjust text with $ sign, and add 'X' at the end
            _boostHintTextAmount.text = $"{GameManager.Instance.GetBoostHintCount()}X";
        }
        else
        {
            // _boostHint.SetActive(false);
            _ribbonHint.SetActive(true);
            _boostHintTextAmount.gameObject.SetActive(false);
        }

        //if player has enough of add time, show add time boost
        if (GameManager.Instance.GetBoostAddTimeCount() > 0)
        {
            // _boostAddTime.SetActive(true);
            _ribbonAddTime.SetActive(false);
            // Adjust text with $ sign, and add 'X' at the end
            _boostAddTimeTextAmount.text = $"{GameManager.Instance.GetBoostAddTimeCount()}X";
        }
        else
        {
            // _boostAddTime.SetActive(false);
            _ribbonAddTime.SetActive(true);
            _boostAddTimeTextAmount.gameObject.SetActive(false);
        }
    }

    //Delete this after testing
    public void TESTWinGame()
    {
        _gameWon();
    }

    //function to check if player has enough of hints, if yes use one otherwise show popup window
    public void ClickOnUseHint()
    {
        if (_isGameOver)
        {
            return;
        }

        if (GameManager.Instance.GetBoostHintCount() > 0)
        {
            //if player has enough of hints, use one and show difference
            GameManager.Instance.UseBoostHint();
            //show difference
            _boostShowDifference();
            //play hint sound
            SoundManager.PlaySound(SoundType.GAME_CORRECT_HINT, volume: 0.8f);
            _adjustBoosts();
        }
        else
        {
            //buys hint if enough coins
            if (!GameManager.Instance.BuyBoostHint())
            {
                //play not enough coins animation
                _topBarUIAnimator.SetTrigger("NotEnoughCoins");
                //play not enough coins sound
                return;
            }
            SoundManager.PlaySound(SoundType.GAME_CORRECT_HINT, volume: 0.8f);

            //if player has enough of coins, buy hint and show difference
            _boostShowDifference();
            //adjust coinsUI
            _adjustTopBarUI();
        }

    }

    //function boost to add time
    public void ClickOnAddTime()
    {
        if (_isGameOver)
        {
            return;
        }
        if (GameManager.Instance.GetBoostAddTimeCount() > 0)
        {
            //if player has enough of add time, use one and add 60 seconds
            GameManager.Instance.UseBoostAddTime();
            //add 60 seconds
            AddTime();
            //play add time sound
            SoundManager.PlaySound(SoundType.GAME_CORRECT_HINT, volume: 0.8f);
            _adjustBoosts();
        }
        else
        {
            //show popup window
            _buyAddTimeWindow.SetActive(true);
            TogglePause();
        }
    }


    //function to adjust top bar ui texts
    private void _adjustTopBarUI()
    {
        //adjust coins text
        _coinsText.text = GameManager.Instance.GetCoins().ToString();
        //adjust lives text
        _lifeText.text = $"{GameManager.Instance.GetLives()}/{GameManager.Instance.GetMaxLiveConst()}";

        _levelText.text = $"LEVEL {GameManager.Instance.GetLevelID()}";
    }

    // function _showDifference();
    private void _boostShowDifference()
    {

        //get random difference
        // int randomDifference = UnityEngine.Random.Range(0, _differences.Count);
        // //get difference
        // Difference difference = _differences[randomDifference];
        // //show difference
        // difference.gameObject.SetActive(true);

    }

}
