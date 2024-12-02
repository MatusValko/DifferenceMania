using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DifferencesManager : MonoBehaviour
{
    public static DifferencesManager Instance { get; private set; }

    [SerializeField]
    private List<Difference> _differences;

    [SerializeField]
    private int _differencesCount = 0;
    [SerializeField]
    private GameObject[] _bottomCircles;

    [SerializeField]
    private int _foundDifferences = 0;


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
    public string jsonFilePath = "Assets/differencesTest.json";
    [SerializeField]
    private GameObject _firstImage;
    [SerializeField]
    private GameObject _secondImage;

    [SerializeField]
    private GameObject _foundCircle;
    [SerializeField]
    private float _timePerDifference = 20f; // Time allocated per difference (in seconds)
    [SerializeField]
    private TextMeshProUGUI _timerText; // UI Text to display the timer
    [SerializeField]
    private float _totalTime; // Total countdown time
    [SerializeField]
    private bool _isPaused = false;
    private bool _isGameOver = false;


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
            DontDestroyOnLoad(Instance);
        }
    }
    void Start()
    {
        // Load and parse the JSON data

        string jsonContent = File.ReadAllText(jsonFilePath);
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
            // _differences.Add(Difference.CreateDifference(diff.x, diff.y, diff.width, diff.height, _firstImage));
            // _differences.Add(Difference.CreateDifference(diff.x, diff.y, diff.width, diff.height, _secondImage));

            // Difference.CreateDifference(diff.x, diff.y, diff.width, diff.height, _secondImage);

            // CreateCollider(diff.x, diff.y, diff.width, diff.height);

            index++;
        }

        // SHOW FOUND CIRCLES EQUAL TO NUMBER OF DIFFERENCES
        _adjustCircles();

        //GET TOTAL TIME TO SOLVE
        _totalTime = _differencesCount * _timePerDifference;
        _updateTimerUI();

    }

    void Update()
    {
        if (_isGameOver)
            return;

        // Countdown logic
        _totalTime -= Time.deltaTime;

        // Update the timer UI
        _updateTimerUI();

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

    private void _gameOver()
    {
        _isGameOver = true;
        _timerText.text = "00:00"; // Display zero time
        DebugLogger.Log("Game Over!");
        // Trigger other game-over actions (e.g., show Game Over screen)
    }

    private void _gameWon()
    {
        DebugLogger.Log("Found all differences!, Game WON!");
        //MAYBE TEMPORALY FREEZE TIME
        Time.timeScale = 0;
        _isGameOver = true;
        // Trigger other game-over actions (e.g., show Game Over screen)
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
    private void CreateCollider(float x, float y, float width, float height)
    {
        // Create a new GameObject
        Difference colliderObject = new Difference();
        GameObject difference = new GameObject("Difference");
        if (_firstImage != null && _secondImage != null)
        {
            colliderObject.gameObject.transform.SetParent(_firstImage.transform);
            colliderObject.gameObject.transform.SetParent(_secondImage.transform);
        }
        colliderObject.transform.localPosition = new Vector2(x, -y);
        colliderObject.transform.localScale = new Vector2(1, 1);


        // Add a BoxCollider2D component and set its size
        BoxCollider2D boxCollider = colliderObject.gameObject.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(width, height);
        boxCollider.offset = new Vector2(width / 2, -height / 2);

        Button buttonCollider = colliderObject.gameObject.AddComponent<Button>();
        // buttonCollider.onClick.AddListener(Clicked);
        // buttonCollider..AddListener(Clicked);

    }



    public void Clicked(int index)
    {
        // Debug.Log("CLICKED");
        List<Difference> foundDifferences = _differences.FindAll(Difference => Difference.id == index);
        Difference differenceLocal = null;
        GameObject circle = null;
        foreach (var difference in foundDifferences)
        {
            DebugLogger.Log("NASLO DIFF" + difference.id);
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



    }



}
