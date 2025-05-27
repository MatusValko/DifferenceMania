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
    [SerializeField] private Image _profileLevelImage;



    public int GetLives()
    {
        return GameManager.Instance.GetLives();
    }

    public void UpdateLives()
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
    public void UpdateCoins()
    {
        _coinsText.text = $"{GameManager.Instance.GetCoins()}";
    }
    //Update the profile level text and image
    public void UpdateProfileLevel()
    {
        _profileLevelText.text = $"LVL {GameManager.Instance.GetProfileLevel()}";
        _profileLevelImage.sprite = GameManager.Instance.GetProfileImageSprite();
    }

    private void InitializeUI()
    {
        UpdateCoins();
        UpdateLives();
        UpdateProfileLevel();
    }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

    }

    void Start()
    {
        InitializeUI();
        SoundManager.PlayThemeSound(SoundType.MAIN_MENU_THEME); //IF QUICKLY LOADED THIS WILL MAKE SURE THE MUSIC IS PLAYING
    }

    // Update is called once per frame
    void Update()
    {

    }
}
