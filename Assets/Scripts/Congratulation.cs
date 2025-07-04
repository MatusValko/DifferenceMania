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
    [SerializeField] private Button _levelsButton;
    [SerializeField] private GameObject _giftsRoomWindow;
    [SerializeField] private Animator _animator; // Animator for the gift animation
    //array of stars
    [SerializeField] private GameObject[] _stars;

    [SerializeField] private Color32 _normalColor = new Color32(255, 255, 255, 255);
    [SerializeField] private Color32 _disabledColor = new Color32(200, 200, 200, 128);

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
        SoundManager.PlaySound(SoundType.CONGRATULATION_CONFETTI);

        _adjustStars();

        //set text to current level
        _levelText.text = $"<size=100>Level <color=#FAE729>{GameManager.Instance.GetLevelID()}</color></size><size=72> Completed!</size>";


        //AK NEMA GIFT MOZE IST DALEJ/DO MENU INAK DISABLE NA TLACIDLA
        if (GameManager.Instance.GetCurrentWins() + 1 >= GameManager.WINS_NEEDED_TO_GIFT)
        {
            _levelsButton.interactable = false;
            _levelsButton.GetComponent<TextMeshProUGUI>().color = _disabledColor;
            _continueButton.interactable = false;
            _continueButton.GetComponent<Image>().color = _disabledColor;
            _continueButton.GetComponentInChildren<TextMeshProUGUI>().color = _disabledColor;

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

        if (GameManager.Instance.GetCurrentWins() >= GameManager.WINS_NEEDED_TO_GIFT)
        {
            //play sound
            SoundManager.PlaySound(SoundType.CONGRATULATION_GIFT_UNLOCK);
            // Play Gift animation
            _animator.SetTrigger("GiftPopUp");
            // Wait for gift animation
            yield return new WaitForSeconds(2);
            // Open gift window
            _giftsRoomWindow.SetActive(true);

            _levelsButton.interactable = true;
            _levelsButton.GetComponent<TextMeshProUGUI>().color = _normalColor;
            _continueButton.interactable = true;
            _continueButton.GetComponent<Image>().color = _normalColor;
            _continueButton.GetComponentInChildren<TextMeshProUGUI>().color = _normalColor;

            // Set new maximum for gift/reset
            GameManager.Instance.ResetWins();
            _adjustSliderAndTexts();

        }
    }

    //adjust stars and sound according to time
    private void _adjustStars()
    {

        int starsCollected = DifferencesManager.Instance.GotStarsFromLevel;
        if (starsCollected == 3) //check if time is less than 0
        {
            //play sound
            SoundManager.PlaySound(SoundType.CONGRATULATION_3STARS);
            StartCoroutine(_activateStars(3));
            StartCoroutine(_starFallingDown(3));
            DebugLogger.Log("3 stars");

        }
        else if (starsCollected == 2)
        {
            //play sound
            SoundManager.PlaySound(SoundType.CONGRATULATION_2STARS);
            StartCoroutine(_activateStars(2));
            StartCoroutine(_starFallingDown(2));

            DebugLogger.Log("2 stars");
        }
        else if (starsCollected == 1)
        {
            //play sound
            SoundManager.PlaySound(SoundType.CONGRATULATION_1STAR);
            StartCoroutine(_activateStars(1));
            StartCoroutine(_starFallingDown(1));

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
    //wait for 0.5 seconds and then activate next star
    private IEnumerator _activateStars(int stars)
    {
        // DebugLogger.Log("TU SOMM: " + stars);
        for (int i = 0; i < stars; i++)
        {
            _stars[i].SetActive(true);
            //play particle system
            ParticleSystem myParticle = _stars[i].GetComponentInChildren<ParticleSystem>();
            if (myParticle != null)
            {
                myParticle.Emit(5);

            }
            yield return new WaitForSeconds(0.5f); // Wait for 0.5 seconds before activating the next star
            //get particle system from star and play it
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
        if (_slider.value < currentWins && _slider.value < _slider.maxValue)
        {
            //play text animation
            _animator.SetTrigger("ProgressCompletedText");

            while (_slider.value < currentWins && _slider.value < _slider.maxValue)
            {
                _slider.value += fillSpeed * Time.deltaTime;
                // DebugLogger.Log(_slider.value);
                yield return null; // Wait for the next frame
            }
        }
        _slider.value = currentWins;
    }

    private IEnumerator _starFallingDown(int stars)
    {
        yield return new WaitForSeconds(5f);
        // DebugLogger.Log("Falling down little stars");
        //play sound
        //select random star gameobject from array max is from argument
        GameObject randomStar = _stars[Random.Range(0, stars)];
        ParticleSystem myParticle = randomStar.GetComponentInChildren<ParticleSystem>();
        // var mainModule = myParticle.main;
        // mainModule.maxParticles = 30; // Set the maximum number of particles to emit
        myParticle.Emit(3);

        // Restart the coroutine for the next star falling animation
        StartCoroutine(_starFallingDown(stars));
    }

    //FROM BUTON CLICK
    public void ClickOnStar()
    {
        // Play sound when clicking on star
        // SoundManager.PlaySound(SoundType.CONGRATULATION_STAR_CLICK);
        //get gameobject that was clicked
        GameObject clickedStar = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        ParticleSystem myParticle = clickedStar.GetComponentInChildren<ParticleSystem>();
        if (myParticle == null)
        {
            DebugLogger.LogError("Particle system not found on the game object.");
            return;
        }
        myParticle.Emit(3); // Emit 3 particles immediately
    }

    public void ContinueButton()
    {
        int nextLevelID = GameManager.Instance.GetLevelID() + 1;
        // GameManager.Instance.SetLevelID(nextLevelID);
        // SceneManager.LoadScene("Game");
        GameManager.Instance.LoadLevel(nextLevelID); // Reset wins after continuing to the next level
    }
    public void MenuButton()
    {
        // Load main menu scene asynchronously and fetch level data, when done, load main menu
        StartCoroutine(_fetchLevelData());
        GameManager.Instance.SetShowLevelsAfterPlaying(true); // Reset level ID to 1 when going back to main menu
        SceneManager.LoadSceneAsync("MainMenu");
    }

    private IEnumerator _fetchLevelData()
    {
        yield return StartCoroutine(LevelLoader.GetProgressData());

    }
}
