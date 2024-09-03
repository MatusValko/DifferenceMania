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

    public void LoadGame()
    {
        //MAIN MENU - 1
        //LOGIN - 2
        StartCoroutine(LoadAsynchronously(1));
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


        DataPersistenceManager.Instance.LoadGame();



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
        const string echoServer = "https://diff.nconnect.sk";
        UnityWebRequest request = new UnityWebRequest(echoServer);
        using (request)
        {
            request.timeout = 5;
            yield return request.SendWebRequest();

            // NO CONNECTION TO SERVER, CHECK IF INTERNET IS AVAIBLE
            if (request.error != null)
            {
                _errorText = "Can't connect to the game server, please wait.";
                Debug.LogError(_errorText);
                SERVER_ERROR = true;
                yield return StartCoroutine(CheckInternetConnection());

            }
            else
            {
                Debug.Log("Successfully connected to game server");
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
                Debug.LogError(_errorText);
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
        _leftSlider.value = value;
        _rightSlider.value = value;

        _loadingText.text = "Loading " + Math.Round(value * 100) + "%";
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
