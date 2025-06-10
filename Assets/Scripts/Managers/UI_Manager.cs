using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance;

    [SerializeField] private TextMeshProUGUI _livesText;

    [SerializeField] private TextMeshProUGUI _coinsText;

    [SerializeField] private TextMeshProUGUI _profileLevelText;
    [SerializeField] private Image _profileAvatarImage;
    [SerializeField] private Sprite _currentProfileAvatarSprite;

    [Header("Avatars Data")]
    public Sprite[] avatarSprites; // Array to hold avatar sprites
    public ProfileOneAvatarImage[] avatars; // Array to hold avatar GameObjects

    [SerializeField] private ProfileOneAvatarImage _oneAvatarPrefab; // Index of the currently selected avatar
    [SerializeField] private Transform _contentTransform; // Transform for the content area where avatars will be instantiated


    public int GetLives()
    {
        return GameManager.Instance.GetLives();
    }

    public void UpdateLivesUI()
    {
        if (_livesText != null)
        {
            if (GetLives() == GameManager.Instance.GetMaxLiveConst())
            {
                _livesText.text = "FULL";
            }
            else
            {
                _livesText.text = $"{GameManager.Instance.GetLives() + "/" + GameManager.Instance.GetMaxLiveConst()}";

            }
        }
    }
    public void UpdateCoinsUI()
    {
        //if not null
        if (_coinsText != null)
        {
            _coinsText.text = $"{GameManager.Instance.GetCoins()}";
        }
    }
    public void UpdateProfileLevel()
    {
        if (_profileLevelText == null || _profileAvatarImage == null)
        {
            Debug.LogError("Profile UI elements are not assigned.");
            return;
        }
        _profileLevelText.text = $"LVL {GameManager.Instance.GetProfileLevel()}";
        int i = GameManager.Instance.GetSelectedPFP() - 1;
        _currentProfileAvatarSprite = avatarSprites[i];
        _profileAvatarImage.sprite = _currentProfileAvatarSprite;
    }

    public Sprite GetCurrentProfileAvatarSprite()
    {
        int i = GameManager.Instance.GetSelectedPFP() - 1;
        _currentProfileAvatarSprite = avatarSprites[i];
        if (_currentProfileAvatarSprite == null)
        {
            DebugLogger.LogError("Current profile avatar sprite is not set.");
            return null;
        }
        // Return the current profile avatar sprite
        return _currentProfileAvatarSprite;
    }
    private void InitializeUI()
    {
        UpdateCoinsUI();
        UpdateLivesUI();
        UpdateProfileLevel();
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    void Start()
    {
        InitializeUI();
        SoundManager.PlayThemeSound(SoundType.MAIN_MENU_THEME); //IF QUICKLY LOADED THIS WILL MAKE SURE THE MUSIC IS PLAYING
    }

    //on enable
    private void OnEnable()
    {
        InitializeUI();

    }
}
