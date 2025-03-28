using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
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


    [System.Serializable]
    private class DifferenceJSON
    {
        public float x;
        public float y;
        public float width;
        public float height;
    }

    [System.Serializable]
    private class DifferencesData
    {
        public string image_id;
        public List<DifferenceJSON> differences;
    }

    // Set the path to the JSON file/ ONLY FOR TESTING
    public string jsonFilePath = "differencesTest";
    [SerializeField] private GameObject _firstImage;
    // [SerializeField] private BoxCollider2D _firstImageCollider;
    [SerializeField] private GameObject _secondImage;
    // [SerializeField] private BoxCollider2D _secondImageCollider;

    [SerializeField]
    private GameObject _foundCircle;
    [SerializeField] private float _timePerDifference = 20f; // Time allocated per difference (in seconds)




    [SerializeField] private float _totalTime; // Total countdown time
    [SerializeField] private bool _isPaused = false;
    private bool _isGameOver = false;
    [SerializeField] private GameObject _xImage; // Prefab for the "X" image

    [SerializeField] private GameObject _levelCompleted;
    [SerializeField] private GameObject _outOfTime;

    [SerializeField] private GameObject _congratulationWindow;
    [SerializeField] private GameObject _youLoseWindow;
    [SerializeField] private GameObject _youLosePopUpWindow;



    [Header("Top bar UI")]
    [SerializeField] Animator _topBarUIAnimator;
    [SerializeField] private TextMeshProUGUI _timerText; // UI Text to display the timer
    [SerializeField] private TextMeshProUGUI _lifeText;
    [SerializeField] private TextMeshProUGUI _coinsText;

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
            // Debug.LogWarning("DIFFERENCE MANAGER IS INSTANTIATED");
            DebugLogger.LogWarning("DIFFERENCE MANAGER IS INSTANTIATED");
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
        SoundManager.PlayThemeSound(SoundType.GAME_THEME);
        //GET IMAGE COLLIDERS
        // _firstImageCollider = _firstImage.GetComponent<BoxCollider2D>();
        // _secondImageCollider = _secondImage.GetComponent<BoxCollider2D>();

        // Load and parse the JSON data and use try catch block to catch any exceptions
        try
        {
            //use reference from inspector for json file
            TextAsset jsonFile = Resources.Load<TextAsset>(jsonFilePath);
            if (jsonFile == null)
            {
                throw new Exception("JSON file not found at path: " + jsonFilePath);

            }
            string jsonContent = jsonFile.text;


            // string jsonContent = File.ReadAllText(jsonFilePath);
            DifferencesData data = JsonUtility.FromJson<DifferencesData>(jsonContent);
            // Debug.Log(data.image_id);
            // Generate colliders
            int index = 0;
            foreach (var diff in data.differences)
            {
                Difference difference1 = Difference.CreateDifference(diff.x, diff.y, diff.width, diff.height, index, _firstImage);
                _differences.Add(difference1);
                Difference difference2 = Difference.CreateDifference(diff.x, diff.y, diff.width, diff.height, index, _secondImage);
                _differences.Add(difference2);
                _differencesCount++;
                index++;
            }
        }
        catch (Exception ex)
        {
            DebugLogger.LogError("Error loading or parsing JSON data: " + ex.Message);
            //show error window
            _errorWindow.SetActive(true);
            _errorText.text = "Error loading or parsing JSON data: " + ex.Message;
        }


        // string jsonContent = File.ReadAllText(jsonFilePath);
        // DifferencesData data = JsonUtility.FromJson<DifferencesData>(jsonContent);
        // // Debug.Log(data.image_id);
        // // Generate colliders
        // int index = 0;
        // foreach (var diff in data.differences)
        // {
        //     Difference difference1 = Difference.CreateDifference(diff.x, diff.y, diff.width, diff.height, index, _firstImage);
        //     _differences.Add(difference1);
        //     Difference difference2 = Difference.CreateDifference(diff.x, diff.y, diff.width, diff.height, index, _secondImage);
        //     _differences.Add(difference2);
        //     _differencesCount++;
        //     index++;
        // }

        // SHOW FOUND CIRCLES EQUAL TO NUMBER OF DIFFERENCES
        _adjustCircles();

        //GET TOTAL TIME TO SOLVE
        _totalTime = _differencesCount * _timePerDifference;
        _updateTimerUI();
        _adjustBoosts();
        _adjustTopBarUI();
    }

    void Update()
    {
        if (_isGameOver)
            return;

        _timeTick();

        // Detect background clicks
        _click();
    }


    private void _click()
    {
        if (_isPaused)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
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
                    DebugLogger.Log("CLICKED ON DIFFERENCE!");
                    Clicked(difference.id);
                    //play correct click sound
                    SoundManager.PlaySound(SoundType.GAME_CORRECT_CLICK);

                }
            }
            else if (hitCollider.gameObject.CompareTag("Image"))
            {
                DebugLogger.LogWarning("NO DIFFERENCE CLICKED!");
                _takeTime();

                // Instantiate the "X" image at the clicked position
                Vector3 position = new Vector3(mousePosition.x, mousePosition.y, 0);
                Instantiate(_xImage, position, Quaternion.identity, _secondImage.transform);
                //play incorrect click sound with 50% lower volume
                SoundManager.PlaySound(SoundType.GAME_INCORRECT_CLICK, volume: 0.5f);
            }
        }
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

    private void _pauseGame()
    {
        Time.timeScale = 0;  // Pause time
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
        _totalTime = _differencesCount * _timePerDifference;
        _isGameOver = false;
    }

    private void _resumeGame()
    {
        Time.timeScale = 1;  // Resume time
        _isPaused = false;
        DebugLogger.Log("Game Resumed");
    }

    private void _adjustCircles()
    {
        for (int i = 1; i <= _bottomCircles.Length; i++)
        {
            if (i > _differencesCount)
            {
                DebugLogger.Log("Differences: " + _differencesCount);
                _bottomCircles[i - 1].transform.parent.gameObject.SetActive(false);
            }
        }
    }
    private void _adjustCorrectCircles()
    {
        _bottomCircles[_foundDifferences - 1].SetActive(true);
        // for (int i = 1; i <= _bottomCircles.Length; i++)
        // {
        //     if (i < _foundDifferences)
        //     {
        //         DebugLogger.Log("Differences: " + _differencesCount);
        //         _bottomCircles[i - 1].SetActive(true);
        //     }
        // }
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
        SoundManager.PlaySound(SoundType.GAME_WIN);
        //start coroutine to next window, congratulations

        StartCoroutine(_showCongratulation());
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
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        SoundManager.StopTheme();
        SceneManager.LoadScene("MainMenu");
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
    [Conditional("UNITY_EDITOR")]
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
                return;
            }
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
        _lifeText.text = $"{GameManager.Instance.GetLives()}/{GameManager.MAX_LIVES}";
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
