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

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        //CHECK INTERNET CONNECTION
        StartCoroutine(CheckInternetConnection());


        int MAX = 1000;
        for (int i = 0; i < MAX; i++)
        {
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
    }
    // WHY STATIC ublic static IEnumerator CheckInternetConnection()
    public IEnumerator CheckInternetConnection()
    {
        const string echoServer = "https://google.com";

        UnityWebRequest request = new UnityWebRequest(echoServer);
        using (request)
        {
            request.timeout = 5;
            yield return request.SendWebRequest();

            if (request.error != null)
            {
                string text = "No connection to internet";
                Debug.LogError(text);
                _loadingText.text = text;
            }
            else
            {
                Debug.Log("Success");
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
