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
        // GENERATE RANDOM NUMBER BETWEEN  1 AND 10
        Button button = gameObject.GetComponent<Button>();
        button.interactable = false;
        int randomNumber = UnityEngine.Random.Range(1, 11);
        _coinValue.text = randomNumber.ToString();

        // ADD VALUE TO TOTAL COINS OBTAINED DURING GIFTS ROOM 
        _giftBox.SetActive(false);
        _CoinwText.SetActive(true);



    }
}
