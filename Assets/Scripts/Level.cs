using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _levelNumberText; // Level number
    //list of stars gameobjects
    [SerializeField] private GameObject[] _stars; // Stars for the level
    // Level Button
    [SerializeField] private Button _levelButton; // Level button

    //set level number
    public void SetLevelNumber(int levelNumber)
    {
        _levelNumberText.text = levelNumber.ToString();
    }

    //based on the stars collected, set the stars to be active or inactive
    public void SetStars(int starsCollected)
    {
        for (int i = 0; i < _stars.Length; i++)
        {
            if (i < starsCollected)
            {
                _stars[i].SetActive(true);
            }
            else
            {
                _stars[i].SetActive(false);
            }
        }
    }

    //set on click event for level button
    public void SetOnClickEvent(int levelId)
    {
        _levelButton.onClick.AddListener(() => ClickLevelButton(levelId));
    }

    public void SetLockedLevelEvent()
    {
        _levelButton.onClick.AddListener(() => _showLockedText());
    }

    private void _showLockedText()
    {
        //get level select component
        UI_Manager UI_Manager = FindFirstObjectByType<UI_Manager>();
        if (UI_Manager == null)
        {
            DebugLogger.LogWarning("UI_Manager NOT found");
            return;
        }

        UI_Manager.ShowLockedLevelText();
    }

    private void ClickLevelButton(int levelId)
    {
        GameManager.Instance.LoadLevel(levelId);
    }
}
