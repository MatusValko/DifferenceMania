using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;

public class GoTo : MonoBehaviour
{
    public Transform levelSelectTransform;
    public GameObject errorWindow;
    public TextMeshProUGUI errorText;

    void Start()
    {
        StartLoadLevelsTEST();
    }

    public void GoToLoginMenu()
    {
        SceneManager.LoadScene("Login");
    }
    public void GoToLoading()
    {
        SceneManager.LoadScene("Loading");
    }

    //if unity editor, load levels
    // [Conditional("UNITY_EDITOR")]
    public void StartLoadLevelsTEST()
    {
        StartCoroutine(LoadLevelsTEST());
    }

    public IEnumerator LoadLevelsTEST()
    {
        string url = GameManager.API_GET_USER_LEVEL_DATA;
        DebugLogger.Log($"Loading Levels from {url}");
        // string token = GameManager.Instance.GetToken();
        string token = "AifToCoKhxahOugECaV8DTc2twqYN8crQ0jINMPNe8f6c99a";
        DebugLogger.Log($"Loading Token: {token}");


        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.SetRequestHeader("Accept", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            List<EpisodeData> episodes = JsonConvert.DeserializeObject<List<EpisodeData>>(jsonResponse);
            GameManager.Instance.SetEpisodes(episodes);
            DebugLogger.Log($"Setting Episodes");

        }
        else
        {
            DebugLogger.LogError($"Error: {request.error}");
            errorWindow.SetActive(true);
            errorText.text = "Error: " + request.error;
        }
    }

    //reload level select
    //delete all children of level select transform and reload levels
    public void ReloadLevelSelect()
    {
        foreach (Transform child in levelSelectTransform)
        {
            Destroy(child.gameObject);
        }
        //reload levels
        StartLoadLevelsTEST();
    }
}
