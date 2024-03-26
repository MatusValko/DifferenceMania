using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{

    // public GameObject loadingScreen;
    public Slider slider;
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
            // Debug.Log(value);
            slider.value = value;
            yield return null; // Pause the coroutine until the next frame
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;
            Debug.Log(progress);
            yield return null;
        }
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
