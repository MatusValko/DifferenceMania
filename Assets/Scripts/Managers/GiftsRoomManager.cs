using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;



public class GiftsRoomManager : MonoBehaviour
{
    public const int MAXFreeSlotsToUnlock = 3;

    public int FreeSlotsToUnlock = 3;

    public int TotalCoinReward = 0;

    [SerializeField]
    private GameObject _buttons;
    [SerializeField]
    private GameObject _collectAllRewardsButton;
    [SerializeField]
    private GameObject _freeSlots;
    [SerializeField]
    private TextMeshProUGUI _freeSlotsText;
    [SerializeField]
    private GameObject _getMoreSlotsForUnlock;
    [SerializeField]
    private TextMeshProUGUI _rewardCoinText;

    [SerializeField]
    private GameObject _rewardRoom;

    [SerializeField]
    private GameObject _giftRoom;


    [SerializeField]
    private TextMeshProUGUI _lastCardText;
    [SerializeField]
    private int _lastCoinValue;

    public int GiftCounter = 0;

    //(array)list for all gift animators
    [SerializeField] private List<Animator> _giftAnimators = new List<Animator>();

    void OnEnable()
    {
        FreeSlotsToUnlock = MAXFreeSlotsToUnlock;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(playAnimations());
    }

    //iNumerator to play animations on gift boxes
    IEnumerator playAnimations()
    {
        int lastIndex = -1;
        while (true)
        {
            //if the list is less than 2, break the loop
            if (_giftAnimators.Count < 2)
            {
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

    // Update is called once per frame
    void Update()
    {

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
        _freeSlotsText.text = $"<color=yellow>{FreeSlotsToUnlock}</color>  free slots to unlock";
    }

    public void ShowGetMoreSlotsForUnlock()
    {
        _getMoreSlotsForUnlock.SetActive(true);
        // Invoke("HideGetMoreSlotsForUnlock", 2);
    }
    public void SetRewardCoinText(int coinValue)
    {
        _rewardCoinText.text = coinValue.ToString() + "X";
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
    public void Claim2XCoins()
    {
        // SHOW ADVERT AND GIVE 2X COINS
        _lastCoinValue = _lastCoinValue * 2;
        _lastCardText.text = _lastCoinValue.ToString();
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
    public void Claim(int coinValue)
    {
        SwitchRooms();
        // ADD COINS
    }
    public void NoThanks()
    {

        gameObject.SetActive(false);
        // SwitchRooms();
        // ADD COINS
    }

    public void SetLastCardText(TextMeshProUGUI text)
    {
        _lastCardText = text;
    }
    public void SetLastCoinValue(int value)
    {
        _lastCoinValue = value;
    }
    private void CollectAllRewards()
    {

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
        DebugLogger.Log("Not found " + animatorName);

    }
}
