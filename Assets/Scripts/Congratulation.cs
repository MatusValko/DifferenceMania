using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DifferencesManager;

public class Congratulation : MonoBehaviour
{

    // [SerializeField] private const int _winsNeededToGift = 6;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _winsNeededToGiftText;
    [SerializeField] private TextMeshProUGUI _completedXoutOfMaxText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private GameObject _giftsRoomWindow;
    [SerializeField] private Animator _animator; // Animator for the gift animation
    //array of stars
    [SerializeField] private GameObject[] _stars;

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
        //send game won to server
        // GameManager.Instance.SendGameWonToServer(GameManager.Instance.GetLevelID(), GameManager.Instance.GetCurrentWins(), GameManager.Instance.GetTime());
        //play sound confetti

        SoundManager.PlaySound(SoundType.CONGRATULATION_CONFETTI);

        //play sound confetti
        // SoundManager.PlaySound(SoundType.CONGRATULATION_FANFARE);

        var levelData = DifferencesManager.Instance.GetLevelData(); // Ensure GetLevelData() returns a valid type
        int currentTime = DifferencesManager.Instance.GetTime();

        _adjustStars(currentTime, levelData);

        //set text to current level
        _levelText.text = $"<size=100>Level <color=#FAE729>{GameManager.Instance.GetLevelID()}</color></size><size=72> Completed!</size>";


        //AK NEMA GIFT MOZE IST DALEJ/DO MENU INAK DISABLE NA TLACIDLA
        if (GameManager.Instance.GetCurrentWins() + 1 == GameManager.WINS_NEEDED_TO_GIFT)
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

        if (GameManager.Instance.GetCurrentWins() == GameManager.WINS_NEEDED_TO_GIFT)
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
    IEnumerator sendWinToServer()
    {
        //send game won to server
        string url = $"https://diff.nconnect.sk/api/level/{GameManager.Instance.GetLevelID()}/win";
        List<IMultipartFormSection> form = new()
        {
            new MultipartFormDataSection("stars", "3"),
            new MultipartFormDataSection("time", "69"),
        };
        using UnityWebRequest request = UnityWebRequest.Post(url, form);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("WEB REQUEST ERROR:" + request.error);
            // StartCoroutine(ShowError(www.error));
            // ShowErrorWindow(request.error);
        }
        else
        {
            Debug.Log("WEB REQUEST SUCCESS:" + request.downloadHandler.text);
            // ShowSuccessWindow(request.downloadHandler.text);
        }

    }

    //adjust stars and sound according to time
    private void _adjustStars(int time, LevelDataGame levelData)
    {

        int spentTime = levelData.total_time_limit - time;
        DebugLogger.Log("Total Time: " + levelData.total_time_limit + " Time spent: " + spentTime + " 3STARS: " + levelData.threeStar + " 2STARS: " + levelData.twoStar + " 1STAR: " + levelData.oneStar);

        if (spentTime <= levelData.threeStar) //check if time is less than 0
        {
            //play sound
            SoundManager.PlaySound(SoundType.CONGRATULATION_3STARS);
            _activateStars(3);
            DebugLogger.Log("3 stars");
        }
        else if (spentTime <= levelData.twoStar)
        {
            //play sound
            SoundManager.PlaySound(SoundType.CONGRATULATION_2STARS);
            _activateStars(2);
            DebugLogger.Log("2 stars");
        }
        else if (spentTime <= levelData.oneStar)
        {
            //play sound
            SoundManager.PlaySound(SoundType.CONGRATULATION_1STAR);
            _activateStars(1);
            DebugLogger.Log("1 star");
        }
        else
        {
            DebugLogger.Log("ZERO STARS, but completed level...");
            SoundManager.PlaySound(SoundType.CONGRATULATION_1STAR);
            //TODO add sound for 0 stars 
        }
    }

    //activate stars according, take int as argument
    private void _activateStars(int stars)
    {
        for (int i = 0; i < stars; i++)
        {
            _stars[i].SetActive(true);
        }
    }

    private void _adjustSliderAndTexts()
    {
        StartCoroutine(_updateSlider());
    }

    private IEnumerator _updateSlider()
    {
        int currentWins = GameManager.Instance.GetCurrentWins();
        int left = GameManager.WINS_NEEDED_TO_GIFT - currentWins;
        _winsNeededToGiftText.text = $"<color=#FAE729>{left}</color> levels left to get reward";
        _completedXoutOfMaxText.text = $"Completed {currentWins}/{GameManager.WINS_NEEDED_TO_GIFT}";


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
        int nextLevelID = GameManager.Instance.GetLevelID() + 1;
        GameManager.Instance.SetLevelID(nextLevelID);

        SceneManager.LoadScene("Game");
    }
    public void MenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
