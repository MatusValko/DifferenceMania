using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class YouLose : MonoBehaviour
{

    // [SerializeField] private const int _winsNeededToGift = 6;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _winsNeededToGiftText;
    [SerializeField] private TextMeshProUGUI _completedXoutOfMaxText;
    [SerializeField] private Animator _animator;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private GameObject[] _timeToGetStars;


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
        _levelText.text = $"<size=100>Level <color=#FAE729>{GameManager.Instance.GetLevelID()}</color></size>\n<size=72> Not Completed</size>";

        // //play sound confetti
        // SoundManager.PlaySound(SoundType.CONGRATULATION_CONFETTI);
        // //play sound confetti
        // SoundManager.PlaySound(SoundType.CONGRATULATION_FANFARE);

        //prvy krat nastavi slider a texty
        _slider.value = GameManager.Instance.GetCurrentWins();
        _adjustSliderAndTexts();

        //gift glow animation
        _animator.SetTrigger("GiftGlow");
        _animator.SetTrigger("CongratulationsText");

        _showTimeToGetStars();

        yield return null;
    }

    private void _showTimeToGetStars()
    {
        for (int i = 0; i < 3; i++)
        {
            int time = DifferencesManager.Instance.GetTimeForStar(i + 1);
            //convert int time to minutes and seconds
            int minutes = time / 60;
            int seconds = time % 60;
            _timeToGetStars[i].GetComponent<TextMeshProUGUI>().text = $"{minutes:D2}:{seconds:D2}";
            _timeToGetStars[i].SetActive(true);
        }
    }

    private void _adjustSliderAndTexts()
    {
        // StartCoroutine(_updateSlider());
        int currentWins = GameManager.Instance.GetCurrentWins();
        int left = GameManager.WINS_NEEDED_TO_GIFT - currentWins;
        _winsNeededToGiftText.text = $"<color=#FAE729>{left}</color> levels left to get reward";
        _completedXoutOfMaxText.text = $"Completed {currentWins}/{GameManager.WINS_NEEDED_TO_GIFT}";

    }
    public void Repeat()
    {
        SceneManager.LoadScene("Game");
    }
    public void MenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
