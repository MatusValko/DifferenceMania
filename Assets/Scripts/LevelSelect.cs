using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public GameObject LevelSelectWindow;
    public GameObject Content;

    public GameObject EpisodePrefab; // Prefab for the level button
    public GameObject LevelPrefab; // Prefab for the level button
    public GameObject LockedLevelPrefab; // Prefab for the level button

    public void PlayTestLevel()
    {
        SceneManager.LoadScene("Game");
    }

    void Awake()
    {
        StartCoroutine(GenerateLevelsAsync());
    }


    //BUTTON CLICK FROM INSPECTOR
    public void ShowHideWindow()
    {
        if (LevelSelectWindow.activeSelf)
        {
            LevelSelectWindow.SetActive(false);
        }
        else
        {
            LevelSelectWindow.SetActive(true);
            //GO TO TOP OF WINDOW
            Content.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

            //GET CHILDREN OBJECT TO RESIZE WINDOW
            // GetChildrenAsArray();
            // GetHeight();
        }
    }



    //generate levels based on level data from game manager
    public IEnumerator GenerateLevelsAsync()
    {
        // Get the level data from the GameManager
        List<EpisodeData> episodes = GameManager.Instance.GetEpisodes();

        // If no episodes are found get episodes from the server, MAINLY FOR TESTING
        if (episodes == null || episodes.Count == 0)
        {
            DebugLogger.LogError("No episodes found. Please load levels first.");
            yield return StartCoroutine(LevelLoader.GetProgressData());
            // After loading progress data, try to get episodes again
            episodes = GameManager.Instance.GetEpisodes();
            if (episodes == null || episodes.Count == 0)
            {
                yield break;
            }
        }

        // Loop through the level data and create buttons for each level
        foreach (EpisodeData episodeData in episodes)
        {
            // instantiate the episode prefab
            GameObject episodeGO = Instantiate(EpisodePrefab);
            //set parent to content
            episodeGO.transform.SetParent(Content.transform, false);
            Episode episode = episodeGO.GetComponent<Episode>();
            episode.SetName("Episode " + episodeData.id);

            if (GameManager.Instance.GetStarsCollected() >= episodeData.unlock_stars)
            {
                // DebugLogger.Log($"Episode {episodeData.id} unlocked with {GameManager.Instance.GetStarsCollected()} stars");
                episode.SetLockedTextOff();
                episode.SetLockedButtonOFF();

                foreach (LevelData levelData in episodeData.levels)
                {
                    //write all levelData parameters to debug log
                    // DebugLogger.Log($"Level ID: {levelData.id}, Name: {levelData.name}, Stars: {levelData.stars_collected}, Opened: {levelData.opened}");
                    if (levelData.opened || levelData.id == 1)
                    {
                        // Create a new button for the level
                        GameObject levelGO = Instantiate(LevelPrefab);
                        episode.AddLevel(levelGO);
                        Level level = levelGO.GetComponent<Level>();
                        level.SetLevelNumber(levelData.name);
                        level.SetStars(levelData.stars_collected);
                        level.SetOnClickEvent(levelData.id);
                    }
                    else
                    {
                        GameObject lockedLevelGO = Instantiate(LockedLevelPrefab);
                        episode.AddLevel(lockedLevelGO);
                    }


                    // Set the button's text to the level name
                    // newButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Level " + level.id;

                    // Add a listener to the button to load the level when clicked
                    // newButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => LoadLevel(level.id));
                }
            }
            else
            {
                // DebugLogger.Log($"Episode {episodeData.id} locked with {GameManager.Instance.GetStarsCollected()} stars");
                episode.SetLockedText(GameManager.Instance.GetStarsCollected(), episodeData.unlock_stars);
                episode.SetLockedTextOn();
                episode.SetLockedButton(episodeData.unlock_coins);
                foreach (LevelData levelData in episodeData.levels)
                {
                    // Create a new button for the level
                    GameObject levelGO = Instantiate(LockedLevelPrefab);
                    episode.AddLevel(levelGO);
                }
            }
            // episode.SetLevelsHeight();

            //iterate through the levels

            // Create a new button for the level
            // GameObject newButton = Instantiate(Resources.Load("LevelButtonPrefab")) as GameObject;
            // newButton.transform.SetParent(Content.transform, false);

            // Set the button's text to the level name
            // newButton.GetComponentInChildren<Text>().text = "Level " + level.id;

            // Add a listener to the button to load the level when clicked
            // newButton.GetComponent<Button>().onClick.AddListener(() => LoadLevel(level.id));
        }

    }

    private void LoadLevel(int levelId)
    {
        // Load the selected level scene
        SceneManager.LoadScene("Game");
    }





}
