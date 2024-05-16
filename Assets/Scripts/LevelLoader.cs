using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{

    // public GameObject loadingScreen;
    [SerializeField]
    private Slider _leftSlider;
    [SerializeField]
    private Slider _rightSlider;
    public TextMeshProUGUI loadingText;
    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        int MAX = 1000;
        for (int i = 0; i < MAX; i++)
        {
            // slider.value =  i % MAX;
            float value = (float)i / MAX;
            ChangeLoading(value);
            // Debug.Log(value);

            yield return null; // Pause the coroutine until the next frame
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            Debug.Log(progress);
            ChangeLoading(progress);
            yield return null;
        }
    }

    private void ChangeLoading(float value)
    {
        _leftSlider.value = value;
        _rightSlider.value = value;

        loadingText.text = "Loading " + Math.Round(value * 100) + "%";


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
