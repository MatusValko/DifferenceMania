using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Congratulation : MonoBehaviour
{

    // [SerializeField] private const int _winsNeededToGift = 6;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _winsNeededToGiftText;
    [SerializeField] private TextMeshProUGUI _completedXoutOfMaxText;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private GameObject _giftsRoomWindow;
    [SerializeField] private Animator _animator; // Animator for the gift animation

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
        //play sound confetti
        SoundManager.PlaySound(SoundType.CONGRATULATION_CONFETTI);
        //play sound confetti
        SoundManager.PlaySound(SoundType.CONGRATULATION_FANFARE);


        //AK NEMA GIFT MOZE IST DALEJ/DO MENU INAK DISABLE NA TLACIDLA
        if (GameManager.Instance.GetCurrentWins() + 1 == GameManager.WINS_NEED_TO_GIFT)
        {
            _menuButton.interactable = false;
            _continueButton.interactable = false;
        }
        //prvy krat nastavi slider a texty
        _slider.value = GameManager.Instance.GetCurrentWins();
        _adjustSliderAndTexts();
        GameManager.Instance.AddWin();

        yield return new WaitForSeconds(1);
        //gift glow animation
        _animator.SetTrigger("GiftGlow");
        _animator.SetTrigger("CongratulationsText");

        //Wait for confetti
        yield return new WaitForSeconds(2.4f);
        SoundManager.PlaySound(SoundType.CONGRATULATION_GIFT_BAR);
        //update slider
        _adjustSliderAndTexts();

        //Wait for animation
        yield return new WaitForSeconds(1);

        if (GameManager.Instance.GetCurrentWins() == GameManager.WINS_NEED_TO_GIFT)
        {
            //play sound
            SoundManager.PlaySound(SoundType.CONGRATULATION_GIFT_UNLOCK);
            // Play Gift animation
            _animator.SetTrigger("GiftPopUp");
            // Wait for gift animation
            yield return new WaitForSeconds(2);
            // Open gift window
            _giftsRoomWindow.SetActive(true);

            _menuButton.interactable = true;
            _continueButton.interactable = true;
            // Set new maximum for gift/reset
            GameManager.Instance.ResetWins();
            _adjustSliderAndTexts();

        }
    }

    private void _adjustSliderAndTexts()
    {
        StartCoroutine(_updateSlider());
    }

    private IEnumerator _updateSlider()
    {
        int currentWins = GameManager.Instance.GetCurrentWins();
        int left = GameManager.WINS_NEED_TO_GIFT - currentWins;
        _winsNeededToGiftText.text = $"<color=#FAE729>{left}</color> levels left to get reward";
        _completedXoutOfMaxText.text = $"Completed {currentWins}/{GameManager.WINS_NEED_TO_GIFT}";


        float fillSpeed = 1f;
        if (_slider.value < currentWins)
        {
            //play text animation
            _animator.SetTrigger("ProgressCompletedText");

            while (_slider.value < currentWins)
            {
                _slider.value += fillSpeed * Time.deltaTime;
                // DebugLogger.Log(_slider.value);
                yield return null; // Wait for the next frame
            }
        }
        _slider.value = currentWins;
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
