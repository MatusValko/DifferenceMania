using System;
using System.Collections;
using TMPro;
using UnityEngine;
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
    private bool SERVER_ERROR = false;

    public void LoadGame()
    {
        //LOGIN - 1
        //MAIN MENU - 2
        StartCoroutine(LoadAsynchronously(1));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {


        //CHECK INTERNET CONNECTION
        StartCoroutine(CheckServerConnection());
        // if (SERVER_ERROR)
        // {
        //     Debug.LogWarning("ERRRRRORRR");
        //     StartCoroutine(CheckInternetConnection());
        //     yield break;
        // }
        // else
        // {
        int MAX = 1000;
        for (int i = 0; i < MAX; i++)
        {
            if (SERVER_ERROR)
            {
                yield break;
            }
            // slider.value =  i % MAX;
            float value = (float)i / MAX;
            ChangeLoading(value);
            // Debug.Log(value);

            yield return null; // Pause the coroutine until the next frame
        }

        //LOAD LEVEL 
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            Debug.Log(progress);
            ChangeLoading(progress);
            yield return null;
        }

        // }


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
                string text = "Can't connect to the game servers, please wait.";
                Debug.LogError(text);
                _loadingText.text = text;
                SERVER_ERROR = true;
                StartCoroutine(CheckInternetConnection());

            }
            else
            {
                Debug.Log("Success");
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
                string text = "No internet connection";
                Debug.LogError(text);
                _loadingText.text = text;
                // SERVER_ERROR = true;

            }
            else
            {
                string text = "Can't connect to the game servers, please wait.";
                Debug.LogError("INTERNET CONNECTION " + text);
                _loadingText.text = text;
            }
        }
    }

    private void ChangeLoading(float value, string text = null)
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
