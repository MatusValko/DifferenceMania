using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance;

    [SerializeField]
    private TextMeshProUGUI _livesText;

    [SerializeField]
    private TextMeshProUGUI _coinsText;

    public int GetLives()
    {
        return GameManager.Instance.GetLives();
    }

    public void UpdateLives()
    {
        if (GetLives() == GameManager.MAX_LIVES)
        {
            _livesText.text = "FULL";
        }
        else
        {
            _livesText.text = $"{GameManager.Instance.GetLives() + "/" + GameManager.MAX_LIVES}";

        }
    }
    public void UpdateCoins()
    {
        _coinsText.text = $"{GameManager.Instance.GetCoins()}";
    }

    private void InitializeUI()
    {
        UpdateCoins();
        UpdateLives();
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
