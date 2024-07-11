using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftBox : MonoBehaviour
{
    [SerializeField]
    private GameObject _giftBox;

    [SerializeField]
    private GameObject _CoinwText;

    [SerializeField]
    private TextMeshProUGUI _coinValue;

    [SerializeField]
    private GiftsRoomManager _giftsRoomManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ClickOnBox()
    {
        if (_giftsRoomManager.FreeSlotsToUnlock == 0)
        {
            //SHOW POP UP
            _giftsRoomManager.ShowGetMoreSlotsForUnlock();
            return;
        }
        _giftsRoomManager.FreeSlotsToUnlock -= 1;
        _giftsRoomManager.UpdateFreeSlotsText();


        _giftsRoomManager.GiftCounter += 1;

        if (_giftsRoomManager.FreeSlotsToUnlock == 0)
        {
            _giftsRoomManager.ShowButtonsHideText();
        }



        // GENERATE RANDOM NUMBER BETWEEN  1 AND 10
        Button button = gameObject.GetComponent<Button>();
        button.interactable = false;
        int randomNumber = UnityEngine.Random.Range(1, 11);
        _coinValue.text = randomNumber.ToString();

        _giftsRoomManager.SetLastCoinValue(randomNumber);
        _giftsRoomManager.SetLastCardText(_coinValue);

        _giftsRoomManager.SwitchRooms();
        _giftsRoomManager.SetRewardCoinText(randomNumber);
        _giftsRoomManager.TotalCoinReward += randomNumber;

        // ADD VALUE TO TOTAL COINS OBTAINED DURING GIFTS ROOM 
        _giftBox.SetActive(false);
        _CoinwText.SetActive(true);



    }
}
