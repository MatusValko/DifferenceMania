using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftBox : MonoBehaviour
{
    [SerializeField] private GameObject _giftBox;

    [SerializeField] private Sprite _giftBoxBestRewardBackground;
    [SerializeField] private GameObject _giftBoxBestReward;
    [SerializeField] private Image _giftBoxBestRewardIcon;
    [SerializeField] private TextMeshProUGUI _giftBoxBestRewardText;

    [SerializeField] private GameObject _coinwText;

    [SerializeField] private TextMeshProUGUI _coinValue;

    [SerializeField] private GiftsRoomManager _giftsRoomManager;

    [SerializeField] private bool _isBestReward = false;

    public void ClickOnBox()
    {
        if (_giftsRoomManager.FreeSlotsToUnlock == 0)
        {
            //SHOW POP UP
            _giftsRoomManager.ShowGetMoreSlotsForUnlock();
            return;
        }
        //play sound
        SoundManager.PlaySound(SoundType.GIFT_OPEN);

        //cannot click on box anymore, disable button
        Button button = gameObject.GetComponent<Button>();
        button.interactable = false;


        _giftsRoomManager.FreeSlotsToUnlock -= 1;
        _giftsRoomManager.UpdateFreeSlotsText();
        _giftsRoomManager.GiftCounter += 1;
        if (_giftsRoomManager.FreeSlotsToUnlock == 0)
        {
            _giftsRoomManager.ShowButtonsHideText();
        }
        _giftBox.SetActive(false);
        _giftsRoomManager.RemoveAnimator(_giftBox.transform.parent.name);

        if (_isBestReward)
        {
            _giftBoxBestReward.SetActive(true);
            Image giftBoxImage = gameObject.GetComponent<Image>();
            giftBoxImage.sprite = _giftBoxBestRewardBackground;
            _giftsRoomManager.SwitchRooms();
            _giftsRoomManager.FoundCollections = 1;
            // set that the best reward is claimed
            // _giftsRoomManager.SetBestRewardClaimed(true);
        }
        else
        {
            //COIN REWARD
            int randomNumber = UnityEngine.Random.Range(2, 11);
            _coinValue.text = randomNumber.ToString();
            _giftsRoomManager.TotalCoinReward += randomNumber;
            _coinwText.SetActive(true);
        }



    }
    public void SetGiftAsBestRewardGift(Sprite sprite)
    {
        _isBestReward = true;
        _giftBoxBestRewardIcon.sprite = sprite;
        _giftBoxBestRewardIcon.SetNativeSize();
    }

    public void SetHowManyPiecesFound(int piecesFound)
    {
        _giftBoxBestRewardText.text = $"{piecesFound} " +
            $"{(_giftsRoomManager.FoundCollections == 1 ? "part" : "parts")}";
    }
}
