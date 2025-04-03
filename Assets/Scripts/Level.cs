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
    [SerializeField]
    private GameObject[] _stars; // Stars for the level
    // Level Button
    [SerializeField]
    private Button _levelButton; // Level button
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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
    private void ClickLevelButton(int levelId)
    {
        GameManager.Instance.SetLevelID(levelId);
        //go to game scene
        SceneManager.LoadScene(3);
    }



}
