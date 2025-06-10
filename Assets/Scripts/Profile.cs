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
    [SerializeField] private TextMeshProUGUI _userTournamentsWonText;
    [SerializeField] private TextMeshProUGUI _userTournamentsLostText;
    [SerializeField] private TextMeshProUGUI _userPlayerIDText;

    [Header("Avatars Data")]
    [SerializeField] private GameObject _selectAvatarWindow; // Reference to the avatar selection window, if needed
    public Sprite[] avatarSprites; // Array to hold avatar sprites
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
        _avatarImage.sprite = avatarSprites[GameManager.Instance.GetSelectedPFP() - 1];

        // Set the user level text
        _userLevelText.text = $"LVL {GameManager.Instance.GetProfileLevel()}";

        // Set the user name text
        _userNameText.text = GameManager.Instance.GetNickname();

        // Set the user progress slider text
        _userProgressSliderText.text = $"{GameManager.Instance.GetExperience()}/{GameManager.Instance.GetExperienceToNextLevel()}";

        // TEMPORARY WON LEVELS
        _userTournamentsWonText.text = GameManager.Instance.GetFinishedLevels().ToString();

        // Set the user tournaments lost text
        _userTournamentsLostText.text = "ZERO";

        // Set GUID of the player? TEMPORARY IS SET TO TOKEN
        _userPlayerIDText.text = "Player ID: " + GameManager.Instance.GetToken();
    }

    private void _setUpAvatarImages()
    {
        // Set the avatar image
        avatars = new ProfileOneAvatarImage[avatarSprites.Length];

        // Create avatar GameObjects and assign sprites
        for (int i = 0; i < avatarSprites.Length; i++)
        {
            ProfileOneAvatarImage avatar = Instantiate(_oneAvatarPrefab, _contentTransform);
            // avatar.GetComponent<SpriteRenderer>().sprite = avatarSprites[i];
            avatar.SetAvatarImage(avatarSprites[i]); // Set the first avatar as selected and with coins and AD
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
