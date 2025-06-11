using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Profile : MonoBehaviour
{
    [SerializeField] private GameObject _connectAccount;
    [SerializeField] private GameObject _accountConnected;
    [SerializeField] private Image _avatarImage;
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

    void OnEnable()
    {
        _showAccountConnectedOrConnectAccount();
        _setUpAvatarImages();
        _setUpDisplayDataAboutAvatar();
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

        // Create avatar GameObjects and assign sprites
        for (int i = 0; i < length; i++)
        {
            ProfileOneAvatarImage avatar = Instantiate(_oneAvatarPrefab, _contentTransform);
            // avatar.GetComponent<SpriteRenderer>().sprite = avatarSprites[i];
            avatar.SetAvatarImage(GameManager.Instance.GetAvatarSprite(i)); // Set the first avatar as selected and with coins and AD


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
                    avatar.SetAvatarBuyWCoins(69);

                }
                else
                {
                    // If the avatar is not unlocked, set it as not selected and with AD
                    avatar.SetAvatarBuyWithAD();
                }
                // If the avatar is not unlocked, set it as not selected and without coins and AD
            }
            avatars[i] = avatar;
        }
    }



    //FROM BUTTON CLICK
    public void ShowAvatarSelectWindow()
    {
        _selectAvatarWindow.SetActive(true);
        _contentTransform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }
}
