using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Congratulation : MonoBehaviour
{

    [SerializeField] private const int _winsNeededToGift = 6;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _winsNeededToGiftText;
    [SerializeField] private TextMeshProUGUI _completedXoutOfMaxText;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private GameObject _GiftsRoomWindow;





    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnEnable()
    {
        StartCoroutine(_setUpWindow());
    }

    private IEnumerator _setUpWindow()
    {
        //AK NEMA GIFT MOZE IST DALEJ/DO MENU INAK DISABLE NA TLACIDLA
        if (GameManager.Instance.CurrentWins + 1 == _winsNeededToGift)
        {
            _menuButton.interactable = false;
            _continueButton.interactable = false;
        }

        _AdjustSliderAndTexts();
        //Wait for confetti
        yield return new WaitForSeconds(2);

        GameManager.Instance.AddWin();
        //play slider animation
        //Wait for animation
        yield return new WaitForSeconds(1);
        _AdjustSliderAndTexts();

        if (GameManager.Instance.CurrentWins == _winsNeededToGift)
        {
            // play Gift animation
            //Wait for gift animation
            yield return new WaitForSeconds(2);
            // open gift window
            _GiftsRoomWindow.SetActive(true);

            _menuButton.interactable = true;
            _continueButton.interactable = true;
            // set new maximum for gift/reset
            GameManager.Instance.ResetWins();
            _AdjustSliderAndTexts();

        }
    }

    private void _AdjustSliderAndTexts()
    {
        //SLIDER TO GIFT
        _slider.value = GameManager.Instance.CurrentWins;
        int left = _winsNeededToGift - GameManager.Instance.CurrentWins;
        _winsNeededToGiftText.text = $"<color=#FAE729>{left}</color> levels left to get reward";
        _completedXoutOfMaxText.text = $"Completed {GameManager.Instance.CurrentWins}/{_winsNeededToGift}";
    }

    public void ContinueButton()
    {
        SceneManager.LoadScene("Game");
    }
    public void MenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }


}
