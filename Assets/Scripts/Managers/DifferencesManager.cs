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

    [SerializeField] private Camera _camera;


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

    [Header("Scroll Rect")]
    [SerializeField] private ScrollRect _scrollRect1;
    [SerializeField] private ScrollRect _scrollRect2;


    [Header("Mobile Input")]
    private Vector2 _startPos;
    private float _startTime;
    private bool _isDragging = false;
    private float lastPinchDistance = 0f;


    private Vector3 _dragOrigin;


    [SerializeField] private float _clickTimeThreshold = 0.2f; // seconds
    [SerializeField] private float _moveThreshold = 10f; // pixels
    [SerializeField] private float _smoothSpeed = 0.1f; // Higher = faster zooming, lower = smoother
    [SerializeField] private float zoomSpeed = 0.02f;

    [SerializeField] private float maxZoom = 6;
    [SerializeField] private float minZoom = 1;

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
    // public LevelDataGame GetLevelData()
    // {
    //     return _levelData;
    // }

    public int GetTimeForStar(int starIndex)
    {
        switch (starIndex)
        {
            case 1:
                return _levelData.oneStar;
            case 2:
                return _levelData.twoStar;
            case 3:
                return _levelData.threeStar;
            default:
                DebugLogger.LogError("Invalid star index: " + starIndex);
                return -1; // Invalid index
        }

    }
    //get time, convert to int
    public int GetTime()
    {
        return (int)_totalTime;
    }

    IEnumerator SetUpGame(int levelId)
    {
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

        // _mobileInput();
        _handleInput();
        // _handleScrollRects();
        // _click();
    }

    private void _click(Vector2 mousePosition)
    {
        if (_isPaused || !_isLoaded)
        {
            return;
        }
        Collider2D[] hitCollider = Physics2D.OverlapPointAll(mousePosition);
        if (hitCollider == null)
        {
            DebugLogger.Log("CLICKED OUTSIDE!");
            return;
        }
        bool inImage1Border = false;
        bool inImage2Border = false;
        Difference difference = null;

        bool clickedOnDifference = false;
        foreach (Collider2D hit in hitCollider)
        {
            if (hit.CompareTag("Image1Border"))
            {
                inImage1Border = true;
                DebugLogger.Log("inImage1Border = true");
            }
            else if (hit.CompareTag("Image2Border"))
            {
                inImage2Border = true;
                DebugLogger.Log("inImage2Border = true");

            }
            else if (hit.CompareTag("Difference"))
            {
                clickedOnDifference = true;
                difference = hit.GetComponent<Difference>();  // Get the Difference component of the clicked object
                DebugLogger.Log("clickedOnDifference = true");

            }

        }
        if (!inImage1Border && !inImage2Border)
        {
            DebugLogger.Log("CLICKED OUTSIDE!");
            return;
        }
        else if (!clickedOnDifference)
        {
            if (inImage1Border) _incorrectClick(mousePosition, true);
            else if (inImage2Border) _incorrectClick(mousePosition, false);
            return; // Exit if no difference was clicked
        }
        else if (clickedOnDifference)
        {
            //ak difference ma parent 1 a klikol som aj na border 1 tak 
            if (difference == null)
            {
                DebugLogger.LogError("Difference component is null!");
                return;
            }
            if (inImage1Border && difference.transform.parent.name == "Image1")
            {
                DebugLogger.Log("CLICKED ON DIFFERENCE IN IMAGE 1!");
                Clicked(difference.id);
                //play correct click sound
                SoundManager.PlaySound(SoundType.GAME_CORRECT_CLICK);
            }
            else if (inImage2Border && difference.transform.parent.name == "Image2")
            {
                DebugLogger.Log("CLICKED ON DIFFERENCE IN IMAGE 2!");
                Clicked(difference.id);
                //play correct click sound
                SoundManager.PlaySound(SoundType.GAME_CORRECT_CLICK);
            }
            else
            {
                DebugLogger.LogWarning("Clicked on difference but not in the correct image border!");
                if (inImage1Border)
                {
                    _incorrectClick(mousePosition, true);

                }
                else if (inImage2Border)
                {
                    _incorrectClick(mousePosition, false);

                }
            }





            // bool firstImageIntesected = false;
            // bool secondImageIntesected = false;

            // Collider2D[] hitAllColliders = Physics2D.OverlapPointAll(mousePosition);
            // foreach (Collider2D hit in hitAllColliders)
            // {
            //     DebugLogger.Log("Overlapping: " + hit.name);
            //     if (hit.CompareTag("Image1Border"))
            //     {
            //         firstImageIntesected = true;
            //         DebugLogger.Log("Intersected 1 Image Border");
            //     }
            //     else if (hit.CompareTag("Image2Border"))
            //     {
            //         secondImageIntesected = true;
            //         DebugLogger.Log("Intersected 2 Image Border");
            //     }


            // }


            // Difference difference = hitCollider.gameObject.GetComponent<Difference>();  // Get the Difference component of the clicked object
            // if (difference != null)
            // {
            //     // DebugLogger.Log("CLICKED ON DIFFERENCE!");
            //     Clicked(difference.id);
            //     //play correct click sound
            //     SoundManager.PlaySound(SoundType.GAME_CORRECT_CLICK);

            // }
        }
        // else if (hitCollider.gameObject.CompareTag("Image"))
        // {
        //     // DebugLogger.LogWarning("NO DIFFERENCE CLICKED!");
        //     _takeTime();

        //     // Instantiate the "X" image at the clicked position
        //     Vector3 position = new Vector3(mousePosition.x, mousePosition.y, 0);
        //     // Quaternion rotation = Quaternion.identity; //OLD
        //     Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        //     if (hitCollider.gameObject.name == "Image1")
        //     {
        //         Instantiate(_xImage, position, rotation, _firstImage.transform);
        //     }
        //     else
        //     {
        //         Instantiate(_xImage, position, rotation, _secondImage.transform);
        //     }
        //     //play incorrect click sound with 50% lower volume
        //     SoundManager.PlaySound(SoundType.GAME_INCORRECT_CLICK, volume: 0.6f);
        //     //TODO vibrate the phone

        // }
        // }
    }

    private void _incorrectClick(Vector2 mousePosition, bool inImage1Border)
    {
        DebugLogger.Log("NO DIFFERENCE CLICKED!");
        _takeTime();

        // Instantiate the "X" image at the clicked position
        Vector3 position = new Vector3(mousePosition.x, mousePosition.y, 0);
        // Quaternion rotation = Quaternion.identity; //OLD
        Quaternion rotation = Quaternion.Euler(0f, 0f, 0f);
        if (inImage1Border)
        {
            Instantiate(_xImage, position, rotation, _firstImage.transform);
        }
        else
        {
            Instantiate(_xImage, position, rotation, _secondImage.transform);
        }
        //play incorrect click sound with 50% lower volume
        SoundManager.PlaySound(SoundType.GAME_INCORRECT_CLICK, volume: 0.6f);


        // Take time from the total time
        _totalTime -= 5f; // Decrease time by 5 seconds for incorrect clicks
        if (_totalTime < 0)
        {
            _totalTime = 0;
        }
        _updateTimerUI();
    }
    public bool _isZooming = false;

    [Header("Custom Scroll Setup - Dual Images")]
    public RectTransform maskRect1; // First mask/viewport area
    public RectTransform maskRect2; // Second mask/viewport area
    public RectTransform contentRect1; // Parent of first image
    public RectTransform contentRect2; // Parent of second image
    public float dragSensitivity = 2f; // Increased from 1f to 2f for faster dragging
    public float mouseZoomSpeed = 1000f;

    private Vector2 _lastMousePos;
    private bool _mousePressed = false;

    // Combined mobile and mouse input
    private void _handleInput()
    {
        // Mobile input
        if (Input.touchCount == 1)
        {
            _handleSingleTouch();
        }
        else if (Input.touchCount == 2)
        {
            _handlePinchZoom();
        }
        else if (Input.touchCount < 2 && _isZooming)
        {
            _handleZoomToTouchTransition();
        }

        // Mouse input (for testing on PC)
        if (Input.touchCount == 0) // Only use mouse when no touches
        {
            _handleMouseInput();
        }
    }

    private void _handleSingleTouch()
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
                if (_isZooming)
                {
                    _isZooming = false;
                    _startPos = touch.position; // Reset start position for dragging
                }
                if (Vector2.Distance(touch.position, _startPos) > _moveThreshold || _isDragging)
                {
                    Vector2 dragDelta = (touch.position - _startPos) * dragSensitivity;
                    _applyDrag(dragDelta * 0.5f); //ADDED 0.1F IT WAS TOO FAST
                    _startPos = touch.position;
                    _isDragging = true;
                }
                break;

            case TouchPhase.Ended:
                float duration = Time.time - _startTime;
                if (!_isDragging && duration < _clickTimeThreshold)
                {
                    Vector2 worldPos = _camera.ScreenToWorldPoint(touch.position);
                    _click(worldPos);
                }
                _isDragging = false;
                _isZooming = false;
                break;
        }
    }

    private void _handlePinchZoom()
    {
        _isDragging = false;

        Touch touch0 = Input.GetTouch(0);
        Touch touch1 = Input.GetTouch(1);
        float currentDistance = Vector2.Distance(touch0.position, touch1.position);

        if (!_isZooming)
        {
            _isZooming = true;
            lastPinchDistance = currentDistance;
        }
        else
        {
            float deltaDistance = currentDistance - lastPinchDistance;
            lastPinchDistance = currentDistance;

            if (Mathf.Abs(deltaDistance) > 2f)
            {
                _applyZoom(deltaDistance * 1.5f); //ADDED 1.5F FOR FASTER ZOOM
            }
        }
    }
    // Add this field to your class (alongside other private fields like _isDragging, _isZooming, etc.)
    // private Vector2 _initialPinchCenter;

    // private void _handlePinchZoom()
    // {
    //     _isDragging = false;

    //     Touch touch0 = Input.GetTouch(0);
    //     Touch touch1 = Input.GetTouch(1);
    //     float currentDistance = Vector2.Distance(touch0.position, touch1.position);

    //     // Calculate the midpoint between the two fingers
    //     Vector2 pinchCenter = (touch0.position + touch1.position) * 0.5f;

    //     if (!_isZooming)
    //     {
    //         _isZooming = true;
    //         lastPinchDistance = currentDistance;
    //         // Store the initial pinch center for consistent zooming
    //         _initialPinchCenter = pinchCenter;
    //     }
    //     else
    //     {
    //         float deltaDistance = currentDistance - lastPinchDistance;
    //         lastPinchDistance = currentDistance;

    //         if (Mathf.Abs(deltaDistance) > 2f)
    //         {
    //             _applyZoomToPoint(deltaDistance * 1.5f, pinchCenter);
    //         }
    //     }
    // }

    // private void _applyZoomToPoint(float zoomDelta, Vector2 screenPoint)
    // {
    //     // Get the mask (parent of the zoom objects) - this is our reference frame
    //     RectTransform maskRect = contentRect1.parent as RectTransform; // This should be UpperImage or LowerImage

    //     // Convert screen point to local coordinates in the mask space
    //     Vector2 localPointInMask;
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //         maskRect, screenPoint, null, out localPointInMask);

    //     // Store current values
    //     float currentScale = contentRect1.localScale.x;
    //     Vector2 currentPos1 = contentRect1.anchoredPosition;
    //     Vector2 currentPos2 = contentRect2.anchoredPosition;

    //     // Calculate new scale using your existing logic
    //     float scaleMultiplier = currentScale * 0.5f;
    //     float scaleChange = zoomDelta * zoomSpeed * 0.01f * scaleMultiplier;
    //     float newScale = Mathf.Clamp(currentScale + scaleChange, minZoom, maxZoom);

    //     // Only proceed if scale actually changed
    //     if (Mathf.Abs(newScale - currentScale) < 0.001f) return;

    //     // Calculate where the zoom point is in the unscaled content space
    //     // This accounts for the current position and scale of each zoom object
    //     Vector2 zoomPointInContent1 = (localPointInMask - currentPos1) / currentScale;
    //     Vector2 zoomPointInContent2 = (localPointInMask - currentPos2) / currentScale;

    //     // Apply the new scale to both zoom objects
    //     contentRect1.localScale = new Vector3(newScale, newScale, 1);
    //     contentRect2.localScale = new Vector3(newScale, newScale, 1);

    //     // Calculate new positions so the zoom point stays at the same location in mask space
    //     Vector2 newPos1 = localPointInMask - (zoomPointInContent1 * newScale);
    //     Vector2 newPos2 = localPointInMask - (zoomPointInContent2 * newScale);

    //     // Apply new positions
    //     contentRect1.anchoredPosition = newPos1;
    //     contentRect2.anchoredPosition = newPos2;

    //     // Ensure content stays within proper bounds
    //     _ensureContentInBounds();
    // }

    // private void _applyZoomToPoint(float zoomDelta, Vector2 screenPoint)
    // {
    //     // Convert screen point to world coordinates before zoom
    //     Vector3 worldPointBeforeZoom = Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, Camera.main.nearClipPlane));

    //     // Apply the zoom (modify your existing zoom logic here)
    //     float zoomFactor = zoomDelta > 0 ? 1.1f : 0.9f; // Adjust as needed
    //     Camera.main.orthographicSize = Mathf.Clamp(
    //         Camera.main.orthographicSize / zoomFactor,
    //         minZoom,
    //         maxZoom
    //     );

    //     // Convert the same screen point to world coordinates after zoom
    //     Vector3 worldPointAfterZoom = Camera.main.ScreenToWorldPoint(new Vector3(screenPoint.x, screenPoint.y, Camera.main.nearClipPlane));

    //     // Calculate the difference and adjust camera position
    //     Vector3 offset = worldPointBeforeZoom - worldPointAfterZoom;
    //     Camera.main.transform.position += offset;
    // }


    private void _handleZoomToTouchTransition()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            _startPos = touch.position;
            _isDragging = true;
        }

        _isZooming = false;
        lastPinchDistance = 0f;
    }

    private void _handleMouseInput()
    {
        // Mouse dragging
        if (Input.GetMouseButtonDown(0))
        {
            _lastMousePos = Input.mousePosition;
            _mousePressed = true;
            _isDragging = false;
            _startTime = Time.time;
        }
        else if (Input.GetMouseButton(0) && _mousePressed)
        {
            Vector2 currentMousePos = Input.mousePosition;
            Vector2 mouseDelta = (currentMousePos - _lastMousePos) * dragSensitivity;

            if (mouseDelta.magnitude > _moveThreshold || _isDragging)
            {
                _applyDrag(mouseDelta);
                _isDragging = true;
            }

            _lastMousePos = currentMousePos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (_mousePressed && !_isDragging && (Time.time - _startTime) < _clickTimeThreshold)
            {
                Vector2 worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
                _click(worldPos);
            }
            _mousePressed = false;
            _isDragging = false;
        }

        // Mouse scroll wheel zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // Convert scroll to distance delta (simulate pinch) - MUCH FASTER
            float zoomDelta = scroll * mouseZoomSpeed; // Use the public field
            _applyZoom(zoomDelta);
        }
    }

    // Shared methods for both touch and mouse
    private void _applyDrag(Vector2 dragDelta)
    {
        Vector2 newPosition1 = contentRect1.anchoredPosition + dragDelta;
        Vector2 newPosition2 = contentRect2.anchoredPosition + dragDelta;

        newPosition1 = ClampToBounds(newPosition1, maskRect1, contentRect1);
        newPosition2 = ClampToBounds(newPosition2, maskRect2, contentRect2);

        contentRect1.anchoredPosition = newPosition1;
        contentRect2.anchoredPosition = newPosition2;
    }

    private void _applyZoom(float deltaDistance)
    {
        float currentScale = contentRect1.localScale.x;
        float scaleMultiplier = currentScale * 0.5f;
        float scaleChange = deltaDistance * zoomSpeed * 0.01f * scaleMultiplier;
        float newScale = Mathf.Clamp(currentScale + scaleChange, minZoom, maxZoom);

        contentRect1.localScale = new Vector3(newScale, newScale, 1);
        contentRect2.localScale = new Vector3(newScale, newScale, 1);

        // After scaling, ensure content stays within proper bounds
        _ensureContentInBounds();
    }

    // New method to ensure content is always properly positioned and visible
    private void _ensureContentInBounds()
    {
        // Fix content 1
        Vector2 pos1 = _getClampedPosition(contentRect1, maskRect1);
        contentRect1.anchoredPosition = pos1;

        // Fix content 2
        Vector2 pos2 = _getClampedPosition(contentRect2, maskRect2);
        contentRect2.anchoredPosition = pos2;
    }

    private Vector2 _getClampedPosition(RectTransform content, RectTransform mask)
    {
        Vector2 contentSize = content.sizeDelta * content.localScale.x;
        Vector2 maskSize = mask.sizeDelta;
        Vector2 currentPos = content.anchoredPosition;

        // If content is smaller than mask, center it
        if (contentSize.x <= maskSize.x)
        {
            currentPos.x = 0; // Center horizontally
        }
        else
        {
            // Content is larger, clamp to bounds
            float maxX = (contentSize.x - maskSize.x) * 0.5f;
            currentPos.x = Mathf.Clamp(currentPos.x, -maxX, maxX);
        }

        if (contentSize.y <= maskSize.y)
        {
            currentPos.y = 0; // Center vertically
        }
        else
        {
            // Content is larger, clamp to bounds
            float maxY = (contentSize.y - maskSize.y) * 0.5f;
            currentPos.y = Mathf.Clamp(currentPos.y, -maxY, maxY);
        }

        return currentPos;
    }

    // Updated ClampToBounds to use the new logic
    private Vector2 ClampToBounds(Vector2 position, RectTransform mask, RectTransform content)
    {
        Vector2 contentSize = content.sizeDelta * content.localScale.x;
        Vector2 maskSize = mask.sizeDelta;

        // If content is smaller than mask, center it
        if (contentSize.x <= maskSize.x)
        {
            position.x = 0;
        }
        else
        {
            float maxX = (contentSize.x - maskSize.x) * 0.5f;
            position.x = Mathf.Clamp(position.x, -maxX, maxX);
        }

        if (contentSize.y <= maskSize.y)
        {
            position.y = 0;
        }
        else
        {
            float maxY = (contentSize.y - maskSize.y) * 0.5f;
            position.y = Mathf.Clamp(position.y, -maxY, maxY);
        }

        return position;
    }

    [Header("OLD Scroll Rect")]
    private float zoomVelocity = 0f;
    void zoom(float increment)
    {
        float currentScale = _firstImage.transform.parent.localScale.x;
        float targetScale = Mathf.Clamp(currentScale + increment, minZoom, maxZoom);
        float smoothScale = Mathf.SmoothDamp(currentScale, targetScale, ref zoomVelocity, _smoothSpeed);

        _firstImage.transform.parent.localScale = new Vector3(smoothScale, smoothScale, 0);
        _secondImage.transform.parent.localScale = new Vector3(smoothScale, smoothScale, 0);
    }
    private void _mobileInput()
    {
        // Handle mobile touch
        if (Input.touchCount == 1)
        {
            _isZooming = false;

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _startPos = touch.position;
                    _startTime = Time.time;
                    _isDragging = false;
                    // _dragOrigin = _camera.ScreenToWorldPoint(touch.position);

                    break;

                case TouchPhase.Moved:
                    if (_isZooming)
                    {
                        _isZooming = false;
                        // _scrollRect1.enabled = true;
                        // _scrollRect2.enabled = true;
                        _startPos = touch.position;
                    }
                    if (Vector2.Distance(touch.position, _startPos) > _moveThreshold)
                    {
                        _isDragging = true;
                        _scrollRect1.enabled = true;
                        _scrollRect2.enabled = true;
                    }
                    break;

                case TouchPhase.Ended:
                    float duration = Time.time - _startTime;
                    if (!_isDragging && duration < _clickTimeThreshold)
                    {
                        Vector2 worldPos = _camera.ScreenToWorldPoint(touch.position);
                        _click(worldPos);
                    }
                    break;
            }
        }
        else if (Input.touchCount == 2) //before was == 2
        {
            _isZooming = true;

            _scrollRect1.enabled = false;
            _scrollRect2.enabled = false;
            // _scrollRect2.normalizedPosition = _scrollRect1.normalizedPosition;//THIS MAKES SURE THAT THEY ARE T HE SAME EVEN WHEN BUGGING

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
            float deltaMagnitudeDiff = currentMagnitude - prevMagnitude;

            if (Mathf.Abs(deltaMagnitudeDiff) > 1f) // Threshold in pixels
            {
                zoom(deltaMagnitudeDiff * zoomSpeed);
            }
            _scrollRect1.enabled = true;
            _scrollRect2.enabled = true;
        }
        else if (Input.touchCount == 0)
        {
            _isZooming = false;
            _scrollRect1.enabled = true;
            _scrollRect2.enabled = true;
        }

        // Handle mouse input (desktop/web)
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        if (Input.GetMouseButtonDown(0))
        {
            DebugLogger.Log("BUTTON DOWN");
            _startPos = Input.mousePosition;
            _startTime = Time.time;
            _isDragging = false;
        }
        else if (Input.GetMouseButton(0))
        {
            if (Vector2.Distance((Vector2)Input.mousePosition, _startPos) > _moveThreshold)
            {
                _isDragging = true;
                DebugLogger.Log("DRAGGING");
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            DebugLogger.Log("BUTTON UP");

            float duration = Time.time - _startTime;
            if (!_isDragging && duration < _clickTimeThreshold)
            {
                DebugLogger.Log("CLICK");
                Vector2 worldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
                _click(worldPos);
            }
        }
#endif
    }

    // void zoom(float increment)
    // {
    //     float currentScale = _firstImage.transform.parent.localScale.x;
    //     float newScale = Mathf.Clamp(currentScale + increment, minZoom, maxZoom);

    //     _firstImage.transform.parent.localScale = new Vector3(newScale, newScale, 1);
    //     _secondImage.transform.parent.localScale = new Vector3(newScale, newScale, 1);
    // }


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
