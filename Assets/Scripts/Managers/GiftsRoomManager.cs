using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class GiftsRoomManager : MonoBehaviour
{
    public const int MAXFreeSlotsToUnlock = 3;

    public int FreeSlotsToUnlock = 3;

    public int TotalCoinReward = 0;
    public int CollectionBestRewardIndex = -1;
    public int FoundCollections = -1;

    [SerializeField] private GameObject _buttons;
    [SerializeField] private GameObject _collectAllRewardsButton;
    [SerializeField] private GameObject _freeSlots;
    [SerializeField] private GameObject _getMoreSlotsForUnlock;
    [SerializeField] private GameObject _rewardRoom;
    [SerializeField] private GameObject _giftRoom;
    [SerializeField] private Image _bestRewardImageSmall;
    [SerializeField] private Image _bestRewardImageBig;

    [SerializeField] private GiftBox _giftBoxBestReward;

    public int GiftCounter = 0;

    //(array)list for all gift animators
    [SerializeField] private List<Animator> _giftAnimators = new List<Animator>();

    [SerializeField] private Sprite[] _collectionItemSprites;

    void Start()
    {
        FreeSlotsToUnlock = MAXFreeSlotsToUnlock;

        _setUpGiftBoxes();
        StartCoroutine(_playAnimations());
    }

    private void _setUpGiftBoxes()
    {
        //select one random gift box to be the best reward
        if (_giftAnimators.Count == 0)
        {
            DebugLogger.LogError("No gift animators found in GiftsRoomManager");
            return;
        }
        int randomIndex = Random.Range(0, _giftAnimators.Count);
        _giftBoxBestReward = _giftAnimators[randomIndex].GetComponent<GiftBox>();

        if (_giftBoxBestReward == null)
        {
            DebugLogger.LogError("No GiftBox component found on animator at index: " + randomIndex);
            return;
        }

        //Select random sprite from collection item sprites
        CollectionBestRewardIndex = Random.Range(0, _collectionItemSprites.Length);
        Sprite bestRewardSprite = _collectionItemSprites[CollectionBestRewardIndex];
        // Set the sprite of the best reward image
        _bestRewardImageSmall.sprite = bestRewardSprite;
        _bestRewardImageSmall.SetNativeSize();
        _bestRewardImageBig.sprite = bestRewardSprite;
        _bestRewardImageBig.SetNativeSize();
        _giftBoxBestReward.SetGiftAsBestRewardGift(bestRewardSprite);


        //TODO: Set up gift boxes to have values inside them

    }

    private IEnumerator _playAnimations()
    {
        int lastIndex = -1;
        while (true)
        {
            //if the list is less than 2, break the loop
            if (_giftAnimators.Count < 2)
            {
                DebugLogger.Log("Not enough animators to play animations: " + _giftAnimators.Count);
                break;
            }
            //pick random animator from array and play animation trigger
            int randomIndex = Random.Range(0, _giftAnimators.Count);
            if (lastIndex != randomIndex)
            {
                lastIndex = randomIndex;
                _giftAnimators[randomIndex].SetTrigger("GiftShake");
                yield return new WaitForSeconds(2);
            }
        }
    }

    public void ShowButtonsHideText()
    {

        if (FreeSlotsToUnlock == 0)
        {
            if (GiftCounter == 9)
            {
                _collectAllRewardsButton.SetActive(true);
                _freeSlots.SetActive(false);
                _buttons.SetActive(false);
            }
            else
            {
                _freeSlots.SetActive(false);
                _buttons.SetActive(true);
            }
        }
        else
        {
            _freeSlots.SetActive(true);
            _buttons.SetActive(false);
        }

    }
    public void UpdateFreeSlotsText()
    {
        //get the text component from _freeSlots
        if (_freeSlots == null)
        {
            DebugLogger.LogError("Free slots text is not assigned in GiftsRoomManager.");
            return;
        }
        TextMeshProUGUI _freeSlotsText = _freeSlots.GetComponentInChildren<TextMeshProUGUI>();
        if (FreeSlotsToUnlock <= 0)
        {
            _freeSlotsText.text = "No free slots to unlock";
            return;
        }
        // Update the text to show the number of free slots to unlock
        _freeSlotsText.text = $"<color=yellow>{FreeSlotsToUnlock}</color>  free slots to unlock";
    }

    public void ShowGetMoreSlotsForUnlock()
    {
        _getMoreSlotsForUnlock.SetActive(true);
        // Invoke("HideGetMoreSlotsForUnlock", 2);
    }

    public void SwitchRooms()
    {
        if (_rewardRoom.activeInHierarchy)
        {
            _rewardRoom.SetActive(false);
            _giftRoom.SetActive(true);

        }
        else
        {
            _rewardRoom.SetActive(true);
            _giftRoom.SetActive(false);
        }
    }
    public void Claim2XCollection()
    {
        // SHOW ADVERT AND GIVE 2X COLLECTIONS
        FoundCollections = 2;
        _giftBoxBestReward.SetHowManyPiecesFound(FoundCollections);
        SwitchRooms();
    }

    public void Get3MoreSlots(int coinValue)
    {
        // SHOW ADVERT AND GIVE 3 MORE SLOT CHANCES
        FreeSlotsToUnlock = MAXFreeSlotsToUnlock;
        UpdateFreeSlotsText();
        ShowButtonsHideText();
        // SwitchRooms();
    }
    public void Claim()
    {
        SwitchRooms();
    }
    // public void NoThanks()
    // {

    //     gameObject.SetActive(false);
    //     // SwitchRooms();
    //     // ADD COINS
    // }

    //FROM BUTTONS
    public void CollectAllRewardsAndQuit()
    {
        //get all rewards from gift boxes
        // TODO SEND COINS TO GAME MANAGER AND SERVER
        GameManager.Instance.AddCoins(TotalCoinReward);
        DebugLogger.Log("Collected all rewards: " + TotalCoinReward + " coins");
        // TODO SEND BEST REWARD TO GAME MANAGER AND SERVER
        //for found collections, add them to the game manager
        for (int i = 0; i < FoundCollections; i++)
        {
            // GameManager.Instance.AddCollection(CollectionBestRewardIndex);
            // DebugLogger.Log("Collected collection: " + i);
        }
        gameObject.SetActive(false);
    }



    public void RemoveAnimator(string animatorName)
    {
        // animatorName += " (Animator)";
        List<Animator> animatorsToRemove = new List<Animator>();
        foreach (var animator in _giftAnimators)
        {
            if (animator != null && animator.name == animatorName)
            {
                //stop animation
                animator.enabled = false;
                animatorsToRemove.Add(animator);
                DebugLogger.Log("Deleted " + animatorName);
            }
        }
        foreach (var animator in animatorsToRemove)
        {
            _giftAnimators.Remove(animator);
        }
        // DebugLogger.Log("Not found " + animatorName);

    }
}
