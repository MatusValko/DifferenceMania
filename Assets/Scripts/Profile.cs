using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    [SerializeField] private GameObject _connectAccount;
    [SerializeField] private GameObject _accountConnected;
    [SerializeField] private Image _avatarImage;
    [SerializeField] private Image _avatarBackgroundImage;

    [SerializeField] private TextMeshProUGUI _userLevelText;
    [SerializeField] private TextMeshProUGUI _userNameText;
    [SerializeField] private TextMeshProUGUI _userProgressSliderText;
    [SerializeField] private Slider _progressSlider; // Slider to show user progress
    [SerializeField] private TextMeshProUGUI _userTournamentsWonText;
    [SerializeField] private TextMeshProUGUI _userTournamentsLostText;
    [SerializeField] private TextMeshProUGUI _userPlayerIDText;

    [Header("Avatars Data")]
    [SerializeField] private GameObject _selectAvatarWindow; // Reference to the avatar selection window, if needed
    // public Sprite[] avatarSprites; // Array to hold avatar sprites
    public ProfileOneAvatarImage[] avatars; // Array to hold avatar GameObjects

    [SerializeField] private ProfileOneAvatarImage _oneAvatarPrefab; // Index of the currently selected avatar
    [SerializeField] private Transform _contentTransform; // Transform for the content area where avatars will be instantiated
    [SerializeField] private Canvas _coinsCanvas; // Button to open the avatar selection window

    void OnEnable()
    {
        _showAccountConnectedOrConnectAccount();
        _setUpAvatarImages();
        _setUpDisplayDataAboutAvatar();
        // _resetAnimationOnButtons();
        // StartCoroutine(_resetAnimationOnButtons());
        // ForceRefreshButtonStates();
    }



    // private void _resetAnimationOnButtons()
    // {
    //     foreach (var animator in GetComponentsInChildren<Animator>())
    //     {
    //         animator.keepAnimatorStateOnDisable = true;
    //         animator.Rebind();
    //         animator.Update(0f);
    //         DebugLogger.Log(animator.name);
    //     }
    // }

    // private IEnumerator _resetAnimationOnButtons()
    // {
    //     yield return null; // Wait one frame

    //     foreach (var animator in GetComponentsInChildren<Animator>(true))
    //     {
    //         // animator.Rebind();
    //         // animator.Update(0f);
    //         //enabling keepAnimatorControllerStateOnDisable
    //         animator.keepAnimatorStateOnDisable = true;
    //         DebugLogger.Log(animator.name);

    //     }
    // }

    public void ForceRefreshButtonStates()
    {
        foreach (var button in GetComponentsInChildren<UIButtonWithSound>(true))
        {
            // ExecuteEvents.Execute<IPointerExitHandler>(button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
            // ExecuteEvents.Execute<IPointerEnterHandler>(button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
            DebugLogger.Log(button.name);
        }
    }

    private void _showAccountConnectedOrConnectAccount()
    {
        if (GameManager.Instance.ISLOGGEDIN)
        {
            _accountConnected.SetActive(true);
            _connectAccount.SetActive(false);
        }
        else
        {
            _accountConnected.SetActive(false);
            _connectAccount.SetActive(true);
        }
    }


    public void LoadLogin()
    {
        //SCENE 2 LOGIN
        SceneManager.LoadScene(2);
    }


    private void _setUpDisplayDataAboutAvatar()
    {
        // Set the avatar image
        _avatarImage.sprite = GameManager.Instance.GetCurrentProfileAvatarSprite();
        _avatarBackgroundImage.sprite = GameManager.Instance.GetProfileBackgroundSprite(GameManager.Instance.GetSelectedPFP());
        // Set the user level text
        _userLevelText.text = $"LVL {GameManager.Instance.GetProfileLevel()}";

        // Set the user name text
        _userNameText.text = GameManager.Instance.GetNickname();

        // Set the user progress slider text
        _userProgressSliderText.text = $"{GameManager.Instance.GetExperience()}/{GameManager.Instance.GetExperienceToNextLevel()}";
        _progressSlider.maxValue = GameManager.Instance.GetExperienceToNextLevel(); // Set the max value of the slider to 1 for percentage representation
        _progressSlider.value = GameManager.Instance.GetExperience();
        // TEMPORARY WON LEVELS
        _userTournamentsWonText.text = GameManager.Instance.GetFinishedLevels().ToString();

        // Set the user tournaments lost text
        _userTournamentsLostText.text = "ZERO";

        // Set GUID of the player?
        _userPlayerIDText.text = "Player ID: " + GameManager.Instance.GetPlayerID();
    }

    private void _setUpAvatarImages()
    {
        // Set the avatar image
        if (avatars.Length != 0)
        {
            DebugLogger.Log("Avatars are initialized.");
            return;
        }
        int length = GameManager.Instance.GetAvatarSpritesLength();
        avatars = new ProfileOneAvatarImage[length];
        List<int> unlockedPFP = GameManager.Instance.GetUnlockedPFP();
        int? coinPrice = null; // Flag to determine if the avatar can be bought with coins
        // Create avatar GameObjects and assign sprites
        for (int i = 0; i < length; i++)
        {
            ProfileOneAvatarImage avatar = Instantiate(_oneAvatarPrefab, _contentTransform);
            // avatar.GetComponent<SpriteRenderer>().sprite = avatarSprites[i];
            avatar.SetAvatarImage(GameManager.Instance.GetAvatarSprite(i)); // Set the first avatar as selected and with coins and AD
            avatar.SetAvatarBackgroundImage(GameManager.Instance.GetProfileBackgroundSprite(i)); // Set the background image for the avatar
            avatar.SetAvatarIndex(i + 1); // Set the index of the avatar (1-based index)
            if (unlockedPFP.Contains(i + 1))
            {
                // If the avatar is unlocked, set it as owned
                avatar.SetAvatarToOwned();
                // Set the avatar as selected if it is the current profile avatar
                if (GameManager.Instance.GetSelectedPFP() == i + 1)
                {
                    avatar.SetAvatarToBeSelected();
                }
            }
            else
            {
                //NOT UNLOCKED, TODO GET FROM SERVER OR GAME MANAGER
                // SELECT RANDOMLY BUY WITH COINS OR AD
                bool buyWithCoins = Random.Range(0, 2) == 0; // Randomly choose between coins and AD
                if (buyWithCoins)
                {
                    // If the avatar is not unlocked, set it as not selected and with coins

                    coinPrice = 69; // Set the price for the avatar
                    avatar.SetAvatarBuyWCoins(coinPrice.Value); // Set the avatar to be bought with coins

                }
                else
                {
                    // If the avatar is not unlocked, set it as not selected and with AD
                    avatar.SetAvatarBuyWithAD();
                }
                // If the avatar is not unlocked, set it as not selected and without coins and AD
            }
            //add listener to the avatar image button
            avatar.GetComponentInChildren<Button>().onClick.AddListener(() => ClickOnAvatarImage(avatar, coinPrice.HasValue ? coinPrice.Value : -1));
            avatars[i] = avatar;

        }
    }

    //update all avatars
    // public void UpdateAvatars()
    // {
    //     // Clear the content transform
    //     foreach (ProfileOneAvatarImage avatar in avatars)
    //     {
    //         Destroy(child.gameObject);
    //     }
    //     // Reinitialize avatars
    //     _setUpAvatarImages();
    // }

    public void ClickOnAvatarImage(ProfileOneAvatarImage avatar, int price = -1)
    {
        if (avatar == null)
        {
            DebugLogger.LogError("Avatar is null");
            return;
        }
        if (avatar.IsSelected())
        {
            DebugLogger.Log("Avatar already selected: " + avatar.gameObject.name + "ID: " + avatar.GetAvatarIndex());
        }
        else if (avatar.IsOwned())
        {
            avatar.SetAvatarToBeSelected();
            GameManager.Instance.SetSelectedPFP(avatar.GetAvatarIndex());
            DebugLogger.Log("Avatar selected with index: " + avatar.GetAvatarIndex());
            _unselectOtherAvatars(avatar);
            // _setUpDisplayDataAboutAvatar();
        }
        else if (avatar.IsBuyWithCoins())
        {
            if (price < 0)
            {
                DebugLogger.LogError("Price is not set for the avatar: " + avatar.gameObject.name + "ID: " + avatar.GetAvatarIndex());
                return;
            }
            if (GameManager.Instance.HasEnoughCoins(price))
            {
                GameManager.Instance.AddCoins(-price); // Deduct the coins from the player's balance
                GameManager.Instance.SetSelectedPFP(avatar.GetAvatarIndex());
                avatar.SetAvatarToOwned(); // Set the avatar to owned state
                avatar.SetAvatarToBeSelected(); // Set the avatar to selected state
                DebugLogger.Log("Avatar bought with coins: " + avatar.gameObject.name + "ID: " + avatar.GetAvatarIndex());
                _unselectOtherAvatars(avatar); // Unselect other avatars
            }
            else
            {
                DebugLogger.Log("Not enough coins to buy the avatar: " + avatar.gameObject.name + "ID: " + avatar.GetAvatarIndex());
                //play animation
                // GameManager.Instance.PlayNotEnoughCoinsAnimation();
            }
            //TODO BUY WITH COINS
            // If the avatar is not owned, buy with coins
        }
        else if (avatar.IsBuyWithAD())
        {
            //TODO BUY WITH AD
            // GameManager.Instance.BuyAvatarWithAD(gameObject.name);
        }
        else
        {
            DebugLogger.LogError("Avatar is not owned, not buyable with coins or AD: " + avatar.gameObject.name + "ID: " + avatar.GetAvatarIndex());
        }


        //UPDATE UI
        _setUpDisplayDataAboutAvatar();
        StartCoroutine(UI_Manager.Instance.InitializeUI());
    }

    private void _unselectOtherAvatars(ProfileOneAvatarImage selectedAvatar)
    {
        // Unselect all other avatars except the selected one
        foreach (ProfileOneAvatarImage avatar in avatars)
        {
            if (avatar != selectedAvatar && avatar.IsSelected())
            {
                if (avatar.IsSelected())
                {
                    avatar.SetAvatarToOwned(); // Reset the other avatars to owned state
                }
            }
        }
    }



    //FROM BUTTON CLICK
    public void ShowAvatarSelectWindow()
    {
        _selectAvatarWindow.SetActive(true);
        _contentTransform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        //set sorting layer ID for the coins canvas to POPUP WINDOW
        _coinsCanvas.sortingLayerName = "PopUpWindow"; // Set the sorting layer of
        _coinsCanvas.sortingOrder = 1; // Set the order in layer of the
    }
    //BUTTON CLICK TO CLOSE THE AVATAR SELECT WINDOW
    public void HideAvatarSelectWindow()
    {
        _selectAvatarWindow.SetActive(false);
        _coinsCanvas.sortingLayerName = "TopBar"; // Reset the sorting layer of the coins canvas
        _coinsCanvas.sortingOrder = 0; // Reset the order in layer of the coins canvas
    }
}
