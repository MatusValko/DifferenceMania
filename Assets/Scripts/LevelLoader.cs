using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        //OTHERWISE GET DATA FROM LOCAL STORAGE




        //TESTING
        yield return new WaitForSeconds(3);
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
    // WHY STATIC public static IEnumerator CheckInternetConnection()
    public IEnumerator CheckServerConnection()
    {
        UnityWebRequest request = new UnityWebRequest(GameManager.GAMESERVER);
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
            }
        }
    }
    public IEnumerator CheckInternetConnection()
    {
        const string echoServer = "https://google.com";
        UnityWebRequest request = new UnityWebRequest(echoServer);
        using (request)
        {
            request.timeout = 5;
            yield return request.SendWebRequest();

            // NO CONNECTION TO SERVER, CHECK IF INTERNET IS AVAIBLE
            if (request.error != null)
            {
                _errorText = "No internet connection";
                DebugLogger.LogError(_errorText);
                // _loadingText.text = text;
                // SERVER_ERROR = true;

            }
            else
            {
                string text = "No internet connection";
                Debug.LogError(text);
            }
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
