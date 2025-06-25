using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Episode : MonoBehaviour
{

    [SerializeField]
    private Transform Levels; // Parent object for levels
    [SerializeField]
    private TextMeshProUGUI EpisodeNameText; // Text object for episode name
    [SerializeField]
    private TextMeshProUGUI LockedText; // Prefab for level button
    [SerializeField]
    private Button BuyEpisode; // Prefab for locked level button
    [SerializeField]
    private TextMeshProUGUI BuyEpisodePrice; // Prefab for locked level button

    //set parent of levels Parent to have same height as the parent of the levels
    public void SetLevelsParentHeight(float height)
    {
        RectTransform rt = Levels.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, height);
    }
    //set parent of levels to have same height as levels
    public void SetLevelsHeight()
    {
        //get paren object of levels
        RectTransform parentRT = Levels.parent.GetComponent<RectTransform>();
        RectTransform levelsRT = Levels.GetComponent<RectTransform>();
        DebugLogger.Log($"Setting levels height to {levelsRT.sizeDelta.y}");
        parentRT.sizeDelta = new Vector2(levelsRT.sizeDelta.x, levelsRT.sizeDelta.y);
    }

    //set name
    public void SetName(string name)
    {
        EpisodeNameText.text = name;
    }

    //add gameobject to levels parent
    public void AddLevel(GameObject level)
    {
        level.transform.SetParent(Levels, false);
    }
    //set locked button
    public void SetLockedButton(int price)
    {
        BuyEpisode.transform.parent.gameObject.SetActive(true);
        BuyEpisodePrice.text = $"<sprite=\"COIN_ICO_184\" index=0> {price}";

        BuyEpisode.interactable = false; //TODO set from backend data
        //add funciotnality to buy episode button
        BuyEpisode.onClick.RemoveAllListeners();
        BuyEpisode.onClick.AddListener(() =>
        {
            DebugLogger.Log($"Buying episode {price}");
            //TODO: add functionality to buy episode
        });
    }
    public void SetLockedButtonOFF()
    {
        // Set the parent of BuyEpisode to inactive
        BuyEpisode.transform.parent.gameObject.SetActive(false);
    }

    public void SetLockedText(int currentStars, int totalStars)
    {
        string text = $"To unlock the level, you need: <b>{currentStars}/{totalStars}</b><sprite=\"STAR\" index=0>";
        LockedText.text = text;
    }
    //set off the locked text
    public void SetLockedTextOff()
    {
        LockedText.transform.parent.gameObject.SetActive(false);
    }
    public void SetLockedTextOn()
    {
        LockedText.transform.parent.gameObject.SetActive(true);
    }




}
